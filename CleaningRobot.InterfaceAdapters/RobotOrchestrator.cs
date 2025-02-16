using CleaningRobot.Entities.Enums;
using CleaningRobot.InterfaceAdapters.Dto;
using CleaningRobot.InterfaceAdapters.Interfaces;
using CleaningRobot.UseCases.Dto.Input;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Interfaces.Controllers;

namespace CleaningRobot.InterfaceAdapters
{
	public class RobotOrchestrator(
		IRobotController robotController,
		IMapController mapController,
		ICommandController commandController,
		IFileAdapter fileAdapter,
		IJsonAdapter jsonAdapter,
		ILogAdapter logAdapter)
		: IRobotOrchestrator
	{
		private readonly IRobotController _robotController = robotController;
		private readonly IMapController _mapController = mapController;
		private readonly ICommandController _commandController = commandController;
		private readonly IFileAdapter _fileAdapter = fileAdapter;
		private readonly IJsonAdapter _jsonAdapter = jsonAdapter;
		private readonly ILogAdapter _logAdapter = logAdapter;

		private readonly Guid _executionId = Guid.NewGuid();

		public async Task<ExecutionResultDto> ExecuteAsync(string inputFilePath, string outputFilePath)
		{
			try
			{
				ValidateInput(inputFilePath, mustExist: true);
				ValidateInput(outputFilePath);

				var fileContent = await ReadFileAsync(inputFilePath);
				var inputData = await DeserializeAsync(fileContent);

				var createMapTask = CreateMapAsync(inputData);
				var createRobotTask = CreateRobotAsync(inputData);
				var createCommandsTask = CreateCommandsListAsync(inputData);

				await Task.WhenAll(createMapTask, createRobotTask, createCommandsTask);

				await ExecuteCommandsAsync();

				var cells = await GetAllCells();
				var visitedCells = GetVisitedCells(cells);
				var cleanedCells = GetCleanedCells(cells);

				var robot = await GetRobotStatusAsync();

				var resultDto = CreateOutputData(visitedCells, cleanedCells, robot);
				var result = await SerializeAsync(resultDto);

				await SaveOutputAsync(outputFilePath, result, replace: true);

				return new ExecutionResultDto(result, _executionId);
			}
			catch (Exception ex)
			{
				await _logAdapter.ErrorAsync(ex.Message, _executionId);
				return new ExecutionResultDto(ex, _executionId);
			}
		}

		#region Private methods
		private async Task SaveOutputAsync(string outputFilePath, string result, bool replace)
		{
			Trace($"Writing output to file '{outputFilePath}'");

			await _fileAdapter.WriteAsync(outputFilePath, result, replace);

			Trace($"Output written to file '{outputFilePath}'");
		}

		private async Task<string> SerializeAsync(OutputDataDto outputData)
		{
			Trace("Serializing output data");

			var result = await _jsonAdapter.SerializeAsync(outputData);

			Trace($"Output data serialized: {result}");

			return result;
		}

		private OutputDataDto CreateOutputData(IEnumerable<CellStatusDto> visitedCells, IEnumerable<CellStatusDto> cleanedCells, RobotStatusDto robot)
		{
			Trace("Creating output data");

			var outputData = new OutputDataDto
			{
				Visited = [.. visitedCells.Select(c => new PositionDto { X = c.X, Y = c.Y })],
				Cleaned = [.. cleanedCells.Select(c => new PositionDto { X = c.X, Y = c.Y })],
				Final = new RobotPositionDto { X = robot.X, Y = robot.Y, Facing = robot.Facing.ToString() },
				Battery = robot.Battery
			};

			Trace($"Output data created: {outputData}");

			return outputData;
		}

		private async Task CreateCommandsListAsync(InputDataDto inputData)
		{
			Trace("Creating commands list");

			//TODO : Add energy consumption for each command type from configuration
			var energyConsumptions = new Dictionary<string, int>
			{
				[CommandType.TurnLeft.ToString()] = 1,
				[CommandType.TurnRight.ToString()] = 1,
				[CommandType.Advance.ToString()] = 2,
				[CommandType.Back.ToString()] = 3,
				[CommandType.Clean.ToString()] = 5
			};

			var data = new CommandDataDto
			{
				Commands = inputData.Commands,
				EnergyConsumptions = energyConsumptions
			};

			await _commandController.CreateAsync(data, _executionId);

			Trace("Commands list created");
		}

		private async Task CreateMapAsync(InputDataDto inputData)
		{
			Trace("Creating map");

			var data = new MapDataDto
			{
				Map = inputData.Map
			};

			await _mapController.CreateAsync(data, _executionId);

			Trace("Map created");
		}

		private async Task CreateRobotAsync(InputDataDto inputData)
		{
			Trace("Creating robot");

			var data = new RobotDataDto
			{
				X = inputData.Start.X,
				Y = inputData.Start.Y,
				Facing = inputData.Start.Facing,
				Battery = inputData.Battery
			};

			await _robotController.CreateAsync(data, _executionId);

			Trace("Robot created");
		}

		private async Task ExecuteCommandsAsync()
		{
			Trace("Executing all commands");

			await _commandController.ExcecuteAllAsync(_executionId);

			Trace("All commands executed");
		}

		private async Task<RobotStatusDto> GetRobotStatusAsync()
		{
			var robot = await _robotController.GetAsync(_executionId);

			Trace($"Robot status: {robot}");

			return robot;
		}

		private async Task<IEnumerable<CellStatusDto>> GetAllCells()
		{
			var cells = await _mapController.GetAsync(_executionId);

			Trace($"All cells: {cells}");

			return cells.Cells;
		}

		private IEnumerable<CellStatusDto> GetVisitedCells(IEnumerable<CellStatusDto> cells)
		{
			var visitedCells = cells.Where(c => c.State == CellState.Visited);

			Trace($"Visited cells: {visitedCells}");

			return visitedCells;
		}

		private IEnumerable<CellStatusDto> GetCleanedCells(IEnumerable<CellStatusDto> cells)
		{
			var cleanedCells = cells.Where(c => c.State == CellState.Cleaned);

			Trace($"Cleaned cells: {cleanedCells}");

			return cleanedCells;
		}

		private void ValidateInput(string filePath, bool mustExist = false)
		{
			if (_fileAdapter.ValidateInput(filePath, out string? error, mustExist))
			{
				throw new ArgumentException(error);
			}

			Trace($"Input file path validated successfully: {filePath}");
		}

		private async Task<string> ReadFileAsync(string filePath)
		{
			Trace($"Reading file '{filePath}'");

			var fileContent = await _fileAdapter.ReadAsync(filePath);

			Trace($"File '{filePath}' read successfully. Content: {fileContent}");

			return fileContent;
		}

		private async Task<InputDataDto> DeserializeAsync(string fileContent)
		{
			Trace("Deserializing input data");

			var inputData = await _jsonAdapter.DeserializeAsync<InputDataDto>(fileContent);


			Trace($"Input data {fileContent} deserialized: {inputData}");

			return inputData;
		}

		private void Trace(string message)
		{
			_logAdapter.InfoAsync($"{message}", _executionId);
		}

		#endregion
	}
}

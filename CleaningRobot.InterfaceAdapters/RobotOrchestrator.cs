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
		ILogAdapter logAdapter,
		EnergyConsumptionConfigurationDto energyConsumptionConfiguration,
		BackOffStrategiesConfigurationDto backOffStrategiesConfigurationDto)
		: IRobotOrchestrator
	{
		private readonly IRobotController _robotController = robotController;
		private readonly IMapController _mapController = mapController;
		private readonly ICommandController _commandController = commandController;
		private readonly IFileAdapter _fileAdapter = fileAdapter;
		private readonly IJsonAdapter _jsonAdapter = jsonAdapter;
		private readonly ILogAdapter _logAdapter = logAdapter;

		private readonly EnergyConsumptionConfigurationDto _energyConfiguration = energyConsumptionConfiguration;
		private readonly BackOffStrategiesConfigurationDto _backoffConfiguration = backOffStrategiesConfigurationDto;

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
			Info($"Writing output to file '{outputFilePath}'");

			await _fileAdapter.WriteAsync(outputFilePath, result, replace);

			Info($"Output written to file '{outputFilePath}' successfully");
		}

		private async Task<string> SerializeAsync(OutputDataDto outputData)
		{
			Info("Serializing output data");

			var result = await _jsonAdapter.SerializeAsync(outputData);

			Info($"Output data serialized successfully");

			return result;
		}

		private OutputDataDto CreateOutputData(IEnumerable<CellStatusDto> visitedCells, IEnumerable<CellStatusDto> cleanedCells, RobotStatusDto robot)
		{
			Info("Creating output data");

			var outputData = new OutputDataDto
			{
				Visited = visitedCells.Select(c => new PositionDto { X = c.X, Y = c.Y }).ToList(),
				Cleaned = cleanedCells.Select(c => new PositionDto { X = c.X, Y = c.Y }).ToList(),
				Final = new RobotPositionDto { X = robot.X, Y = robot.Y, Facing = robot.Facing.ToString()[..1] },
				Battery = robot.Battery,
			};

			Info($"Output data created successfully");

			return outputData;
		}

		private async Task<CommandDataDto> CreateCommandsListAsync(InputDataDto inputData)
		{
			Info("Creating commands list");

			var energyConsumptions = GetEnergyConsumption();
			var backoffStrategy = GetBackoffStrategy();

			var data = new CommandDataDto
			{
				Commands = inputData.Commands,
				EnergyConsumptions = energyConsumptions,
				ExecutionId = _executionId,
				BackoffStrategy = backoffStrategy,
				ConsumeEnergyWhenBackOff = ConsumeEnergyWhenBackOff,
				StopWhenBackOff = StopWhenBackOff
			};

			var result = await _commandController.CreateAsync(data, _executionId);

			Info($"Commands list created successfully");

			return data;
		}

		private async Task<MapDataDto> CreateMapAsync(InputDataDto inputData)
		{
			Info("Creating map");

			var data = new MapDataDto
			{
				Map = inputData.Map,
				ExecutionId = _executionId
			};

			var result = await _mapController.CreateAsync(data, _executionId);

			Info($"Map created successfully");

			return data;
		}

		private async Task<RobotDataDto> CreateRobotAsync(InputDataDto inputData)
		{
			Info("Creating robot");

			var data = new RobotDataDto
			{
				X = inputData.Start.X,
				Y = inputData.Start.Y,
				Facing = inputData.Start.Facing,
				Battery = inputData.Battery,
				ExecutionId = _executionId
			};

			var result = await _robotController.CreateAsync(data, _executionId);

			Info($"Robot created successfully");

			return data;
		}

		private async Task ExecuteCommandsAsync()
		{
			Info("Executing all commands");

			var result = await _commandController.ExcecuteAllAsync(_executionId);

			Info($"All commands executed successfully");
		}

		private async Task<RobotStatusDto> GetRobotStatusAsync()
		{
			var robot = await _robotController.GetAsync(_executionId);

			Info($"Robot status received successfully");

			return robot;
		}

		private async Task<IEnumerable<CellStatusDto>> GetAllCells()
		{
			var cells = await _mapController.GetAsync(_executionId);

			Info($"All cell received successfully");

			return cells.Cells;
		}

		private IEnumerable<CellStatusDto> GetVisitedCells(IEnumerable<CellStatusDto> cells)
		{
			var visitedCells = cells.Where(c => c.State == CellState.Visited || c.State == CellState.Cleaned);

			Info($"Visited cells received successfully");

			return visitedCells;
		}

		private IEnumerable<CellStatusDto> GetCleanedCells(IEnumerable<CellStatusDto> cells)
		{
			var cleanedCells = cells.Where(c => c.State == CellState.Cleaned);

			Info($"Cleaned cells received successfully");

			return cleanedCells;
		}

		private void ValidateInput(string filePath, bool mustExist = false)
		{
			if (!_fileAdapter.ValidateInput(filePath, out string? error, mustExist))
			{
				throw new ArgumentException(error);
			}

			Info("Input file path validated successfully");
		}

		private async Task<string> ReadFileAsync(string filePath)
		{
			Info($"Reading file '{filePath}'");

			var fileContent = await _fileAdapter.ReadAsync(filePath);

			Info($"File '{filePath}' read successfully");

			return fileContent;
		}

		private async Task<InputDataDto> DeserializeAsync(string fileContent)
		{
			Info("Deserializing input data");

			var inputData = await _jsonAdapter.DeserializeAsync<InputDataDto>(fileContent);


			Info($"Input data deserialized sucessfully");

			return inputData;
		}

		private void Info(string message)
		{
			_logAdapter.InfoAsync($"{message}", _executionId);
		}

		private Dictionary<string, int> GetEnergyConsumption()
		{
			var energyConsumptions = new Dictionary<string, int>
			{
				[CommandType.TurnLeft.ToString()] = _energyConfiguration?.TurnLeft?.EnergyConsumption ?? 1,
				[CommandType.TurnRight.ToString()] = _energyConfiguration?.TurnRight?.EnergyConsumption ?? 1,
				[CommandType.Advance.ToString()] = _energyConfiguration?.Advance?.EnergyConsumption ?? 2,
				[CommandType.Back.ToString()] = _energyConfiguration?.Back?.EnergyConsumption ?? 3,
				[CommandType.Clean.ToString()] = _energyConfiguration?.Clean?.EnergyConsumption ?? 5
			};

			return energyConsumptions;
		}

		private IEnumerable<IEnumerable<string>> GetBackoffStrategy()
		{
			return _backoffConfiguration.Sequences;
		}

		private bool StopWhenBackOff => _backoffConfiguration.StopWhenBackOff;
		private bool ConsumeEnergyWhenBackOff => _backoffConfiguration.ConsumeEnergyWhenBackOff;

		#endregion
	}
}

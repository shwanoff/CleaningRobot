using CleaningRobot.Entities.Enums;
using CleaningRobot.InterfaceAdapters.Dto;
using CleaningRobot.InterfaceAdapters.Interfaces;
using CleaningRobot.UseCases.Dto;
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

		private readonly bool _writeTraceLog = false; // TODO: Get from configuration

		public ExecutionResultDto Execute(string inputFilePath, string outputFilePath)
		{
			try
			{
				ValidateInput(inputFilePath, mustExist: true);
				ValidateInput(outputFilePath);

				var fileContent = ReadFile(inputFilePath);
				var inputData = Deserialize(fileContent);

				CreateMap(inputData);
				CreateRobot(inputData);
				CreateCommandsList(inputData);

				ExecuteCommands(runAll: true);

				var cells = GetAllCells();
				var visitedCells = GetVisitedCells(cells);
				var cleanedCells = GetCleanedCells(cells);

				var robot = GetRobotStatus();

				var resultDto = CreateOutputData(visitedCells, cleanedCells, robot);

				var result = Serialize(resultDto);
				SaveOutput(outputFilePath, result, replace: true);

				return new ExecutionResultDto(result);
			}
			catch (Exception ex)
			{
				_logAdapter.Error(ex.Message);
				return new ExecutionResultDto(ex);
			}
		}

		#region Private methods
		private void SaveOutput(string outputFilePath, string result, bool replace)
		{
			Trace($"Writing output to file '{outputFilePath}'");

			if (!_fileAdapter.TryWrite(outputFilePath, result, replace))
			{
				throw new ArgumentException($"Error writing output to file '{outputFilePath}'");
			}

			Trace($"Output written to file '{outputFilePath}'");
		}

		private string Serialize(OutputDataDto outputData)
		{
			Trace("Serializing output data");

			if (!_jsonAdapter.TrySerialize(outputData, out string result))
			{
				throw new ArgumentException("Error serializing output data");
			}

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

		private void CreateCommandsList(InputDataDto inputData)
		{
			Trace("Creating commands list");

			_commandController.Create(inputData.Commands);

			Trace("Commands list created");
		}

		private void CreateMap(InputDataDto inputData)
		{
			Trace("Creating map");

			_mapController.Create(inputData.Map);

			Trace("Map created");
		}

		private void CreateRobot(InputDataDto inputData)
		{
			Trace("Creating robot");

			_robotController.Create(inputData.Start.X, inputData.Start.Y, inputData.Start.Facing, inputData.Battery);

			Trace("Robot created");
		}

		private void ExecuteCommands(bool runAll)
		{
			if (runAll)
			{
				ExecuteAllCommands();
			}
			else
			{
				ExecuteCommandsOneByOne();
			}
		}

		private void ExecuteAllCommands()
		{
			Trace("Executing all commands");

			_commandController.ExcecuteAll();

			Trace("All commands executed");
		}

		private void ExecuteCommandsOneByOne()
		{
			CommandStatusDto? command;

			do
			{
				command = _commandController.ExecuteNext();

				if (command != null)
				{
					Trace($"Executing next command: {command}");

					GetRobotStatus();

					GetAllCells();
				}
			}
			while (command != null);
		}

		private RobotStatusDto GetRobotStatus()
		{
			var robot = _robotController.GetCurrentStatus();

			Trace($"Robot status: {robot}");

			return robot;
		}

		private IEnumerable<CellStatusDto> GetAllCells()
		{
			var cells = _mapController.GetAllCellStatuses();

			Trace($"All cells: {cells}");

			return cells;
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
			if (string.IsNullOrWhiteSpace(filePath))
			{
				throw new ArgumentException("File path is empty");
			}

			if (!_fileAdapter.IsPath(filePath))
			{
				throw new ArgumentException($"File path '{filePath}' is invalid");
			}

			if (mustExist && !_fileAdapter.Exists(filePath))
			{
				throw new ArgumentException($"File '{filePath}' does not exist");
			}

			Trace($"Input file path validated successfully: {filePath}");
		}

		private string ReadFile(string filePath)
		{
			Trace($"Reading file '{filePath}'");

			if (!_fileAdapter.TryRead(filePath, out string fileContent))
			{
				throw new ArgumentException($"Error reading file '{filePath}'");
			}

			Trace($"File '{filePath}' read successfully. Content: {fileContent}");

			return fileContent;
		}

		private InputDataDto Deserialize(string fileContent)
		{
			Trace("Deserializing input data");

			if (!_jsonAdapter.TryDeserialize(fileContent, out InputDataDto inputData))
			{
				throw new ArgumentException("Error deserializing input data");
			}

			Trace($"Input data {fileContent} deserialized: {inputData}");

			return inputData;
		}

		private void Trace(string message)
		{
			if (_writeTraceLog)
			{
				_logAdapter.Info(message);
			}
		}

		#endregion
	}
}

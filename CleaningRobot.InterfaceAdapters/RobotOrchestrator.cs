using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.InterfaceAdapters.Dto;
using CleaningRobot.InterfaceAdapters.Interfaces;
using CleaningRobot.UseCases.Dto;
using CleaningRobot.UseCases.Interfaces.Controllers;

namespace CleaningRobot.InterfaceAdapters
{
	public class RobotOrchestrator : IRobotOrchestrator
	{
		private readonly IRobotController _robotController;
		private readonly IMapController _mapController;
		private readonly ICommandController _commandController;
		private readonly IFileAdapter _fileAdapter;
		private readonly IJsonAdapter _jsonAdapter;
		private readonly ILogAdapter _logAdapter;


		public RobotOrchestrator(
			IRobotController robotController, 
			IMapController mapController, 
			ICommandController commandController, 
			IFileAdapter fileAdapter, 
			IJsonAdapter jsonAdapter, 
			ILogAdapter logAdapter)
		{
			_robotController = robotController;
			_mapController = mapController;
			_commandController = commandController;
			_fileAdapter = fileAdapter;
			_jsonAdapter = jsonAdapter;
			_logAdapter = logAdapter;
		}

		public ExecutionResult Execute(string inputFilePath, string outputFilePath)
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

				return new ExecutionResult(result);
			}
			catch (Exception ex)
			{
				_logAdapter.Error(ex.Message);
				return new ExecutionResult(ex);
			}
		}

		#region Private methods
		private void SaveOutput(string outputFilePath, string result, bool replace)
		{
			if (!_fileAdapter.TryWrite(outputFilePath, result, replace))
			{
				throw new ArgumentException($"Error writing output to file '{outputFilePath}'");
			}
		}

		private string Serialize(OutputDataDto outputData)
		{
			if (!_jsonAdapter.TrySerialize(outputData, out string result))
			{
				throw new ArgumentException("Error serializing output data");
			}

			return result;
		}

		private OutputDataDto CreateOutputData(IEnumerable<CellStatusDto> visitedCells, IEnumerable<CellStatusDto> cleanedCells, RobotStatusDto robot)
		{
			var outputData = new OutputDataDto
			{
				Visited = visitedCells.Select(c => new PositionDto { X = c.X, Y = c.Y }).ToList(),
				Cleaned = cleanedCells.Select(c => new PositionDto { X = c.X, Y = c.Y }).ToList(),
				Final = new RobotPositionDto { X = robot.X, Y = robot.Y, Facing = robot.Facing.ToString() },
				Battery = robot.Battery
			};

			return outputData;
		}

		private void CreateCommandsList(InputDataDto inputData)
		{
			_commandController.Create(inputData.Commands);
		}

		private void CreateMap(InputDataDto inputData)
		{
			_mapController.Create(inputData.Map);
		}

		private void CreateRobot(InputDataDto inputData)
		{
			_robotController.Create(inputData.Start.X, inputData.Start.Y, inputData.Start.Facing, inputData.Battery);
		}

		private void ExecuteCommands(bool runAll)
		{
			if (runAll)
			{
				_commandController.ExcecuteAll();
			}
			else
			{
				_commandController.ExecuteNext();
			}
		}

		private RobotStatusDto GetRobotStatus()
		{
			var robot = _robotController.GetCurrentStatus();
			return robot;
		}

		private IEnumerable<CellStatusDto> GetAllCells()
		{
			var cells = _mapController.GetAllCellStatuses();
			return cells;
		}

		private IEnumerable<CellStatusDto> GetVisitedCells(IEnumerable<CellStatusDto> cells)
		{
			var visitedCells = cells.Where(c => c.State == CellState.Visited);
			return visitedCells;
		}

		private IEnumerable<CellStatusDto> GetCleanedCells(IEnumerable<CellStatusDto> cells)
		{
			var cleanedCells = cells.Where(c => c.State == CellState.Cleaned);
			return cleanedCells;
		}

		private void ValidateInput(string filePath, bool mustExist = false)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				throw new ArgumentException("File path is empty");
			}

			if (!_fileAdapter.IsFilePath(filePath))
			{
				throw new ArgumentException($"File path '{filePath}' is invalid");
			}

			if (mustExist && !_fileAdapter.Exists(filePath))
			{
				throw new ArgumentException($"File '{filePath}' does not exist");
			}
		}

		private string ReadFile(string filePath)
		{
			if (!_fileAdapter.TryRead(filePath, out string fileContent))
			{
				throw new ArgumentException($"Error reading file '{filePath}'");
			}

			return fileContent;
		}

		private InputDataDto Deserialize(string fileContent)
		{
			if (!_jsonAdapter.TryDeserialize(fileContent, out InputDataDto inputData))
			{
				throw new ArgumentException("Error deserializing input data");
			}

			return inputData;
		}

		#endregion
	}
}

using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.InterfaceAdapters.Dto;
using CleaningRobot.InterfaceAdapters.Interfaces;
using CleaningRobot.UseCases.Dto;
using CleaningRobot.UseCases.Interfaces.Controllers;

namespace CleaningRobot.InterfaceAdapters
{
	/// <summary>
	/// Orchestrates the robot cleaning process
	/// </summary>
	/// <param name="robotController"> Controll robot state </param>
	/// <param name="mapController"> Controll map state </param>
	/// <param name="commandController"> Controll command list state </param>
	/// <param name="fileAdapter"> Controll work with file system </param>
	/// <param name="jsonAdapter"> Controll work with JSON files </param>
	/// <param name="logAdapter"> Controll logging </param>
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

		private readonly bool _writeTraceLog = false;

		/// <summary>
		/// Execute the robot cleaning process
		/// </summary>
		/// <param name="inputFilePath"> Path to input JSON file with map, commands and robot parameters </param>
		/// <param name="outputFilePath"> Path to output JSON file with list of cleaned/visited points and robot status at the end </param>
		/// <returns> Results of execution list of cleaned/visited points and robot status, same as in output file </returns>
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
		/// <summary>
		/// Save output to file
		/// </summary>
		/// <param name="outputFilePath"> Path to the output file </param>
		/// <param name="result"> Serialized result data </param>
		/// <param name="replace"> Flag indicating whether to replace the existing file </param>
		/// <exception cref="ArgumentException"> Thrown when there is an error writing to the file </exception>
		private void SaveOutput(string outputFilePath, string result, bool replace)
		{
			Trace($"Writing output to file '{outputFilePath}'");

			if (!_fileAdapter.TryWrite(outputFilePath, result, replace))
			{
				throw new ArgumentException($"Error writing output to file '{outputFilePath}'");
			}

			Trace($"Output written to file '{outputFilePath}'");
		}

		/// <summary>
		/// Serialize output data
		/// </summary>
		/// <param name="outputData"> Data to be serialized </param>
		/// <returns> Serialized string of output data </returns>
		/// <exception cref="ArgumentException"> Thrown when there is an error during serialization </exception>
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

		/// <summary>
		/// Create output data
		/// </summary>
		/// <param name="visitedCells"> List of cells visited by the robot </param>
		/// <param name="cleanedCells"> List of cells cleaned by the robot </param>
		/// <param name="robot"> Current status of the robot </param>
		/// <returns> Data transfer object containing the output data </returns>
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

		/// <summary>
		/// Create commands list
		/// </summary>
		/// <param name="inputData"> Input data containing the commands </param>
		private void CreateCommandsList(InputDataDto inputData)
		{
			Trace("Creating commands list");

			_commandController.Create(inputData.Commands);

			Trace("Commands list created");
		}

		/// <summary>
		/// Create map
		/// </summary>
		/// <param name="inputData"> Input data containing the map information </param>
		private void CreateMap(InputDataDto inputData)
		{
			Trace("Creating map");

			_mapController.Create(inputData.Map);

			Trace("Map created");
		}

		/// <summary>
		/// Create robot
		/// </summary>
		/// <param name="inputData"> Input data containing the robot's initial position and battery level </param>
		private void CreateRobot(InputDataDto inputData)
		{
			Trace("Creating robot");

			_robotController.Create(inputData.Start.X, inputData.Start.Y, inputData.Start.Facing, inputData.Battery);

			Trace("Robot created");
		}

		/// <summary>
		/// Execute commands
		/// </summary>
		/// <param name="runAll"> Flag indicating whether to run all commands at once or one by one </param>
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

		/// <summary>
		/// Execute all commands at once
		/// </summary>
		private void ExecuteAllCommands()
		{
			Trace("Executing all commands");

			_commandController.ExcecuteAll();

			Trace("All commands executed");
		}

		/// <summary>
		/// Execute commands one by one
		/// </summary>
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

		/// <summary>
		/// Get robot status
		/// </summary>
		/// <returns>Current status of the robot</returns>
		private RobotStatusDto GetRobotStatus()
		{
			var robot = _robotController.GetCurrentStatus();

			Trace($"Robot status: {robot}");

			return robot;
		}

		/// <summary>
		/// Get all cells status
		/// </summary>
		/// <returns>List of all cell statuses</returns>
		private IEnumerable<CellStatusDto> GetAllCells()
		{
			var cells = _mapController.GetAllCellStatuses();

			Trace($"All cells: {cells}");

			return cells;
		}

		/// <summary>
		/// Get visited cells
		/// </summary>
		/// <param name="cells"> List of all cell statuses </param>
		/// <returns> List of cells visited by the robot </returns>
		private IEnumerable<CellStatusDto> GetVisitedCells(IEnumerable<CellStatusDto> cells)
		{
			var visitedCells = cells.Where(c => c.State == CellState.Visited);

			Trace($"Visited cells: {visitedCells}");

			return visitedCells;
		}

		/// <summary>
		/// Get cleaned cells
		/// </summary>
		/// <param name="cells"> List of all cell statuses </param>
		/// <returns> List of cells cleaned by the robot </returns>
		private IEnumerable<CellStatusDto> GetCleanedCells(IEnumerable<CellStatusDto> cells)
		{
			var cleanedCells = cells.Where(c => c.State == CellState.Cleaned);

			Trace($"Cleaned cells: {cleanedCells}");

			return cleanedCells;
		}

		/// <summary>
		/// Validate input data
		/// </summary>
		/// <summary>
		/// Validate input data
		/// </summary>
		/// <param name="filePath"> Path to the file </param>
		/// <param name="mustExist"> Flag indicating whether the file must exist </param>
		/// <exception cref="ArgumentException"> Thrown when the file path is invalid or the file does not exist </exception>
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

		/// <summary>
		/// Read file content
		/// </summary>
		/// <summary>
		/// Read file content
		/// </summary>
		/// <param name="filePath"> Path to the file </param>
		/// <returns> Content of the file as a string </returns>
		/// <exception cref="ArgumentException"> Thrown when there is an error reading the file </exception>
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

		/// <summary>
		/// Deserialize input data
		/// </summary>
		/// <param name="fileContent"> Content of the input file </param>
		/// <returns> Deserialized input data </returns>
		/// <exception cref="ArgumentException"> Thrown when there is an error during deserialization </exception>
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

		/// <summary>
		/// Write trace message
		/// </summary>
		/// <param name="message"> Trace message to be logged </param>
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

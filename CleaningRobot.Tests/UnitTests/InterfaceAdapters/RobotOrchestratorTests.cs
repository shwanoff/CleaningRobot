using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.InterfaceAdapters;
using CleaningRobot.InterfaceAdapters.Dto;
using CleaningRobot.InterfaceAdapters.Interfaces;
using CleaningRobot.UseCases.Dto.Input;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Interfaces.Controllers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleaningRobot.Tests.UnitTests.InterfaceAdapters
{
	[TestFixture]
	public class RobotOrchestratorTests
	{
		private Mock<IRobotController> _robotControllerMock;
		private Mock<IMapController> _mapControllerMock;
		private Mock<ICommandController> _commandControllerMock;
		private Mock<IFileAdapter> _fileAdapterMock;
		private Mock<IJsonAdapter> _jsonAdapterMock;
		private Mock<ILogAdapter> _logAdapterMock;
		private RobotOrchestrator _orchestrator;
		private EnergyConsumptionConfigurationDto _energyConfig;
		private BackOffStrategiesConfigurationDto _backoffConfig;

		[SetUp]
		public void SetUp()
		{
			_robotControllerMock = new Mock<IRobotController>();
			_mapControllerMock = new Mock<IMapController>();
			_commandControllerMock = new Mock<ICommandController>();
			_fileAdapterMock = new Mock<IFileAdapter>();
			_jsonAdapterMock = new Mock<IJsonAdapter>();
			_logAdapterMock = new Mock<ILogAdapter>();

			_energyConfig = new EnergyConsumptionConfigurationDto
			{
				TurnLeft = new CommandConsumption { Command = "TurnLeft", EnergyConsumption = 1 },
				TurnRight = new CommandConsumption { Command = "TurnRight", EnergyConsumption = 1 },
				Advance = new CommandConsumption { Command = "Advance", EnergyConsumption = 2 },
				Back = new CommandConsumption { Command = "Back", EnergyConsumption = 3 },
				Clean = new CommandConsumption { Command = "Clean", EnergyConsumption = 5 }
			};

			_backoffConfig = new BackOffStrategiesConfigurationDto
			{
				Sequences = new List<List<string>> { new List<string> { "A", "B" } },
				ConsumeEnergyWhenBackOff = true,
				StopWhenBackOff = false
			};

			_orchestrator = new RobotOrchestrator(
				_robotControllerMock.Object,
				_mapControllerMock.Object,
				_commandControllerMock.Object,
				_fileAdapterMock.Object,
				_jsonAdapterMock.Object,
				_logAdapterMock.Object,
				_energyConfig,
				_backoffConfig
			);
		}

		[Test]
		public async Task ExecuteAsync_ValidInput_ReturnsSuccess()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var inputFilePath = "input.json";
			var outputFilePath = "output.json";
			var inputData = new InputDataDto
			{
				Map = new string[][] { new string[] { "S", "C" } },
				Start = new RobotPositionDto { X = 0, Y = 0, Facing = "N" },
				Commands = new List<string> { "A", "C" },
				Battery = 100
			};
			var fileContent = "{\"Map\":[[\"S\",\"C\"]],\"Start\":{\"X\":0,\"Y\":0,\"Facing\":\"N\"},\"Commands\":[\"A\",\"C\"],\"Battery\":100}";
			var outputData = new OutputDataDto
			{
				Visited = new List<PositionDto> { new PositionDto { X = 0, Y = 0 } },
				Cleaned = new List<PositionDto> { new PositionDto { X = 0, Y = 0 } },
				Final = new RobotPositionDto { X = 0, Y = 0, Facing = "N" },
				Battery = 90
			};
			var serializedOutput = "{\"Visited\":[{\"X\":0,\"Y\":0}],\"Cleaned\":[{\"X\":0,\"Y\":0}],\"Final\":{\"X\":0,\"Y\":0,\"Facing\":\"N\"},\"Battery\":90}";

			_fileAdapterMock.Setup(f => f.ValidateInput(inputFilePath, out It.Ref<string?>.IsAny, true)).Returns(true);
			_fileAdapterMock.Setup(f => f.ValidateInput(outputFilePath, out It.Ref<string?>.IsAny, false)).Returns(true);
			_fileAdapterMock.Setup(f => f.ReadAsync(inputFilePath)).ReturnsAsync(fileContent);
			_jsonAdapterMock.Setup(j => j.DeserializeAsync<InputDataDto>(fileContent)).ReturnsAsync(inputData);
			_mapControllerMock.Setup(m => m.CreateAsync(It.IsAny<MapDataDto>(), It.IsAny<Guid>())).ReturnsAsync(new MapStatusDto { IsCorrect = true, Cells = new List<CellStatusDto>(), Width = 2, Height = 1, ExecutionId = executionId });
			_robotControllerMock.Setup(r => r.CreateAsync(It.IsAny<RobotDataDto>(), It.IsAny<Guid>())).ReturnsAsync(new RobotStatusDto { IsCorrect = true, X = 0, Y = 0, Facing = Facing.North, Battery = 100, ExecutionId = executionId });
			_commandControllerMock.Setup(c => c.CreateAsync(It.IsAny<CommandDataDto>(), It.IsAny<Guid>())).ReturnsAsync(new CommandCollectionStatusDto { IsCorrect = true, Commands = new List<CommandStatusDto>(), ExecutionId = executionId });
			_commandControllerMock.Setup(c => c.ExcecuteAllAsync(It.IsAny<Guid>())).ReturnsAsync("Success");
			_mapControllerMock.Setup(m => m.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new MapStatusDto { Cells = new List<CellStatusDto> { new CellStatusDto { X = 0, Y = 0, State = CellState.Cleaned, Type = CellType.CleanableSpace, IsCorrect = true, ExecutionId = executionId } }, IsCorrect = true, Width = 2, Height = 1, ExecutionId = executionId });
			_robotControllerMock.Setup(r => r.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new RobotStatusDto { X = 0, Y = 0, Facing = Facing.North, Battery = 90, IsCorrect = true, ExecutionId = executionId });
			_jsonAdapterMock.Setup(j => j.SerializeAsync(It.Is<OutputDataDto>(o => o.Visited.Count == outputData.Visited.Count && o.Cleaned.Count == outputData.Cleaned.Count && o.Final.X == outputData.Final.X && o.Final.Y == outputData.Final.Y && o.Final.Facing == outputData.Final.Facing && o.Battery == outputData.Battery))).ReturnsAsync(serializedOutput);
			_fileAdapterMock.Setup(f => f.WriteAsync(outputFilePath, serializedOutput, true)).Returns(Task.CompletedTask);

			// Act
			var result = await _orchestrator.ExecuteAsync(inputFilePath, outputFilePath);

			// Assert
			Assert.That(result.Success, Is.True);
			Assert.That(result.Result, Is.EqualTo(serializedOutput));
		}

		[Test]
		public async Task ExecuteAsync_CommandExecutionFails_ReturnsError()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var inputFilePath = "input.json";
			var outputFilePath = "output.json";
			var inputData = new InputDataDto
			{
				Map = new string[][] { new string[] { "S", "C" } },
				Start = new RobotPositionDto { X = 0, Y = 0, Facing = "N" },
				Commands = new List<string> { "A", "C" },
				Battery = 100
			};
			var fileContent = "{\"Map\":[[\"S\",\"C\"]],\"Start\":{\"X\":0,\"Y\":0,\"Facing\":\"N\"},\"Commands\":[\"A\",\"C\"],\"Battery\":100}";

			_fileAdapterMock.Setup(f => f.ValidateInput(inputFilePath, out It.Ref<string?>.IsAny, true)).Returns(true);
			_fileAdapterMock.Setup(f => f.ValidateInput(outputFilePath, out It.Ref<string?>.IsAny, false)).Returns(true);
			_fileAdapterMock.Setup(f => f.ReadAsync(inputFilePath)).ReturnsAsync(fileContent);
			_jsonAdapterMock.Setup(j => j.DeserializeAsync<InputDataDto>(fileContent)).ReturnsAsync(inputData);
			_mapControllerMock.Setup(m => m.CreateAsync(It.IsAny<MapDataDto>(), It.IsAny<Guid>())).ReturnsAsync(new MapStatusDto { IsCorrect = true, Cells = new List<CellStatusDto>(), Width = 2, Height = 1, ExecutionId = executionId });
			_robotControllerMock.Setup(r => r.CreateAsync(It.IsAny<RobotDataDto>(), It.IsAny<Guid>())).ReturnsAsync(new RobotStatusDto { IsCorrect = true, X = 0, Y = 0, Facing = Facing.North, Battery = 100, ExecutionId = executionId });
			_commandControllerMock.Setup(c => c.CreateAsync(It.IsAny<CommandDataDto>(), It.IsAny<Guid>())).ReturnsAsync(new CommandCollectionStatusDto { IsCorrect = true, Commands = new List<CommandStatusDto>(), ExecutionId = executionId });
			_commandControllerMock.Setup(c => c.ExcecuteAllAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception("Command execution failed"));

			// Act
			var result = await _orchestrator.ExecuteAsync(inputFilePath, outputFilePath);

			// Assert
			Assert.That(result.Success, Is.False);
			Assert.That(result.Error.Message, Is.EqualTo("Command execution failed"));
		}
	}
}
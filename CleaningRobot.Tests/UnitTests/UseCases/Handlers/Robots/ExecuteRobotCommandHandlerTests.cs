using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Handlers.Robots;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Robots
{
	[TestFixture]
	public class ExecuteRobotCommandHandlerTests
	{
		private Mock<IRepository<Robot>> _robotRepositoryMock;
		private Mock<IQueueRepository<Command>> _commandRepositoryMock;
		private Mock<ILogAdapter> _logAdapterMock;
		private ExecuteRobotCommandHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_robotRepositoryMock = new Mock<IRepository<Robot>>();
			_commandRepositoryMock = new Mock<IQueueRepository<Command>>();
			_logAdapterMock = new Mock<ILogAdapter>();
			_handler = new ExecuteRobotCommandHandler(_robotRepositoryMock.Object, _commandRepositoryMock.Object, _logAdapterMock.Object);
		}

		[Test]
		public async Task Handle_ValidRequest_ReturnsCorrectStatus()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.Clean, 10)
			{
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = true,
				IsCompletedByCommand = true,
				IsCompletedByMap = true,
				IsCompletedByRobot = true,
			};
			var robot = new Robot(1, 1, Facing.North, 100);

			_robotRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(robot);
			_commandRepositoryMock.Setup(repo => repo.UpdateFirstAsync(command, executionId)).ReturnsAsync(command);
			_robotRepositoryMock.Setup(repo => repo.UpdateAsync(robot, executionId)).ReturnsAsync(robot);

			var request = new ExecuteRobotCommand
			{
				ExecutionId = executionId,
				Command = command
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.IsCompleted, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
			Assert.That(result.Result.Battery, Is.EqualTo(90));
		}

		[Test]
		public void Handle_NullCommand_ThrowsArgumentNullException()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var request = new ExecuteRobotCommand
			{
				ExecutionId = executionId,
				Command = null
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(request, CancellationToken.None));
		}

		[Test]
		public void Handle_NegativeEnergyConsumption_ThrowsArgumentException()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.Clean, -10)
			{
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = true,
				IsCompletedByCommand = true,
				IsCompletedByMap = true,
				IsCompletedByRobot = true,
			};
			var request = new ExecuteRobotCommand
			{
				ExecutionId = executionId,
				Command = command
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(request, CancellationToken.None));
		}

		[Test]
		public async Task Handle_TurnLeftCommand_UpdatesRobotFacingAndBattery()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.TurnLeft, 10)
			{
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = true,
				IsCompletedByCommand = true,
				IsCompletedByMap = true,
				IsCompletedByRobot = true,
			};
			var robot = new Robot(1, 1, Facing.North, 100);

			_robotRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(robot);
			_commandRepositoryMock.Setup(repo => repo.UpdateFirstAsync(command, executionId)).ReturnsAsync(command);
			_robotRepositoryMock.Setup(repo => repo.UpdateAsync(robot, executionId)).ReturnsAsync(robot);

			var request = new ExecuteRobotCommand
			{
				ExecutionId = executionId,
				Command = command
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.Result.Facing, Is.EqualTo(Facing.West));
			Assert.That(result.Result.Battery, Is.EqualTo(90));
		}

		[Test]
		public async Task Handle_TurnRightCommand_UpdatesRobotFacingAndBattery()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.TurnRight, 10)
			{
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = true,
				IsCompletedByCommand = true,
				IsCompletedByMap = true,
				IsCompletedByRobot = true,
			};
			var robot = new Robot(1, 1, Facing.North, 100);

			_robotRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(robot);
			_commandRepositoryMock.Setup(repo => repo.UpdateFirstAsync(command, executionId)).ReturnsAsync(command);
			_robotRepositoryMock.Setup(repo => repo.UpdateAsync(robot, executionId)).ReturnsAsync(robot);

			var request = new ExecuteRobotCommand
			{
				ExecutionId = executionId,
				Command = command
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.Result.Facing, Is.EqualTo(Facing.East));
			Assert.That(result.Result.Battery, Is.EqualTo(90));
		}

		[Test]
		public async Task Handle_AdvanceCommand_UpdatesRobotPositionAndBattery()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.Advance, 10)
			{
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = true,
				IsCompletedByCommand = true,
				IsCompletedByMap = true,
				IsCompletedByRobot = true,
			};
			var robot = new Robot(1, 1, Facing.North, 100);

			_robotRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(robot);
			_commandRepositoryMock.Setup(repo => repo.UpdateFirstAsync(command, executionId)).ReturnsAsync(command);
			_robotRepositoryMock.Setup(repo => repo.UpdateAsync(robot, executionId)).ReturnsAsync(robot);

			var request = new ExecuteRobotCommand
			{
				ExecutionId = executionId,
				Command = command
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.Result.X, Is.EqualTo(1));
			Assert.That(result.Result.Y, Is.EqualTo(0));
			Assert.That(result.Result.Battery, Is.EqualTo(90));
		}

		[Test]
		public async Task Handle_BackCommand_UpdatesRobotPositionAndBattery()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.Back, 10)
			{
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = true,
				IsCompletedByCommand = true,
				IsCompletedByMap = true,
				IsCompletedByRobot = true,
			};
			var robot = new Robot(1, 1, Facing.North, 100);

			_robotRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(robot);
			_commandRepositoryMock.Setup(repo => repo.UpdateFirstAsync(command, executionId)).ReturnsAsync(command);
			_robotRepositoryMock.Setup(repo => repo.UpdateAsync(robot, executionId)).ReturnsAsync(robot);

			var request = new ExecuteRobotCommand
			{
				ExecutionId = executionId,
				Command = command
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.Result.X, Is.EqualTo(1));
			Assert.That(result.Result.Y, Is.EqualTo(2));
			Assert.That(result.Result.Battery, Is.EqualTo(90));
		}

		[Test]
		public async Task Handle_CleanCommand_UpdatesRobotBattery()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.Clean, 10)
			{
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = true,
				IsCompletedByCommand = true,
				IsCompletedByMap = true,
				IsCompletedByRobot = true,
			};
			var robot = new Robot(1, 1, Facing.North, 100);

			_robotRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(robot);
			_commandRepositoryMock.Setup(repo => repo.UpdateFirstAsync(command, executionId)).ReturnsAsync(command);
			_robotRepositoryMock.Setup(repo => repo.UpdateAsync(robot, executionId)).ReturnsAsync(robot);

			var request = new ExecuteRobotCommand
			{
				ExecutionId = executionId,
				Command = command
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.Result.Battery, Is.EqualTo(90));
		}
	}
}
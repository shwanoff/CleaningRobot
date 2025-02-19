using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Handlers.Robots;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Robots
{
	[TestFixture]
	public class ValidateCommandByRobotQueryHandlerTests
	{
		private Mock<IRepository<Robot>> _robotRepositoryMock;
		private ValidateCommandByRobotQueryHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_robotRepositoryMock = new Mock<IRepository<Robot>>();
			_handler = new ValidateCommandByRobotQueryHandler(_robotRepositoryMock.Object);
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

			var request = new ValidateCommandByRobotQuery
			{
				ExecutionId = executionId,
				Command = command
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.IsValid, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
		}

		[Test]
		public async Task Handle_NullCommand_ReturnsValidationError()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var request = new ValidateCommandByRobotQuery
			{
				ExecutionId = executionId,
				Command = null
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Error, Is.EqualTo("Command cannot be null"));
			Assert.That(result.State, Is.EqualTo(ResultState.ValidationError));
		}

		[Test]
		public async Task Handle_NegativeEnergyConsumption_ReturnsValidationError()
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
			var request = new ValidateCommandByRobotQuery
			{
				ExecutionId = executionId,
				Command = command
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Error, Is.EqualTo("The energy consumption of a command cannot be negative"));
			Assert.That(result.State, Is.EqualTo(ResultState.ValidationError));
		}

		[Test]
		public async Task Handle_NonExistentExecutionId_ReturnsValidationError()
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
			var request = new ValidateCommandByRobotQuery
			{
				ExecutionId = executionId,
				Command = command
			};

			_robotRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync((Robot)null);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Error, Is.EqualTo($"Robot for execution ID {executionId} not found."));
			Assert.That(result.State, Is.EqualTo(ResultState.ValidationError));
		}

		[Test]
		public async Task Handle_InsufficientBattery_ReturnsOutOfEnergy()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.Clean, 150)
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

			var request = new ValidateCommandByRobotQuery
			{
				ExecutionId = executionId,
				Command = command
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Error, Is.EqualTo($"The battery level is too low. The battery level is {robot.Battery} and the command requires {command.EnergyConsumption}"));
			Assert.That(result.State, Is.EqualTo(ResultState.OutOfEnergy));
		}
	}
}
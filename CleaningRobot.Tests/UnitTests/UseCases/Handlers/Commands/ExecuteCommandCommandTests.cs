using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Handlers.Commands;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Commands
{
	[TestFixture]
	public class ExecuteCommandCommandTests
	{
		private Mock<IQueueRepository<Command>> _commandRepositoryMock;
		private Mock<IBackoffRepository> _backoffRepositoryMock;
		private Mock<ILogAdapter> _logAdapterMock;
		private ExecuteCommandCommandHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_commandRepositoryMock = new Mock<IQueueRepository<Command>>();
			_backoffRepositoryMock = new Mock<IBackoffRepository>();
			_logAdapterMock = new Mock<ILogAdapter>();
			_handler = new ExecuteCommandCommandHandler(_commandRepositoryMock.Object, _backoffRepositoryMock.Object, _logAdapterMock.Object);
		}

		[Test]
		public async Task Handle_ValidRequest_UpdatesCommandAndReturnsCorrectStatus()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.TurnLeft, 1) 
			{ 
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = true,
				IsCompletedByCommand = true,
				IsCompletedByMap = true,
				IsCompletedByRobot = true
			};
			var request = new ExecuteCommandCommand
			{
				ExecutionId = executionId,
				Command = command,
				Backoff = false
			};

			_commandRepositoryMock
				.Setup(repo => repo.UpdateFirstAsync(command, executionId))
				.ReturnsAsync(command);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.IsCompleted, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
			Assert.That(result.Result.Type, Is.EqualTo(CommandType.TurnLeft));
			Assert.That(result.Result.EnergyConsumption, Is.EqualTo(1));
			Assert.That(result.Result.IsValid, Is.True);
			Assert.That(result.Result.IsCompleted, Is.True);
		}

		[Test]
		public async Task Handle_BackoffRequest_UpdatesBackoffCommandAndReturnsCorrectStatus()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.TurnLeft, 1)
			{
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = true,
				IsCompletedByCommand = true,
				IsCompletedByMap = true,
				IsCompletedByRobot = true
			};
			var request = new ExecuteCommandCommand
			{
				ExecutionId = executionId,
				Command = command,
				Backoff = true
			};

			_backoffRepositoryMock
				.Setup(repo => repo.UpdateFirstAsync(command, executionId))
				.ReturnsAsync(command);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.IsCompleted, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
			Assert.That(result.Result.Type, Is.EqualTo(CommandType.TurnLeft));
			Assert.That(result.Result.EnergyConsumption, Is.EqualTo(1));
			Assert.That(result.Result.IsValid, Is.True);
			Assert.That(result.Result.IsCompleted, Is.True);
		}

		[Test]
		public void Handle_NullRequest_ThrowsArgumentNullException()
		{
			// Arrange
			ExecuteCommandCommand request = null;

			// Act & Assert
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(request, CancellationToken.None));
		}

		[Test]
		public void Handle_InvalidCommand_ThrowsArgumentException()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.TurnLeft, -1)
			{
				IsValidatedByCommand = false,
				IsValidatedByMap = false,
				IsValidatedByRobot = false,
			};
			var request = new ExecuteCommandCommand
			{
				ExecutionId = executionId,
				Command = command,
				Backoff = false
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(request, CancellationToken.None));
		}
	}
}
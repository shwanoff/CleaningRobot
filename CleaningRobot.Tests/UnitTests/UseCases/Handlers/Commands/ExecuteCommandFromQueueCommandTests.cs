using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Handlers.Commands;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;
using MediatR;
using CleaningRobot.Entities.Interfaces;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Commands
{
	[TestFixture]
	public class ExecuteCommandFromQueueCommandTests
	{
		private Mock<IQueueRepository<Command>> _commandRepositoryMock;
		private Mock<IMediator> _mediatorMock;
		private Mock<ILogAdapter> _logAdapterMock;
		private ExecuteCommandFromQueueCommandHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_commandRepositoryMock = new Mock<IQueueRepository<Command>>();
			_mediatorMock = new Mock<IMediator>();
			_logAdapterMock = new Mock<ILogAdapter>();
			_handler = new ExecuteCommandFromQueueCommandHandler(_commandRepositoryMock.Object, _mediatorMock.Object, _logAdapterMock.Object);
		}

		[Test]
		public async Task Handle_CommandIsNull_ReturnsQueueIsEmpty()
		{
			// Arrange
			var request = new ExecuteCommandFromQueueCommand { ExecutionId = Guid.NewGuid() };
			_commandRepositoryMock.Setup(repo => repo.PeekAsync(request.ExecutionId)).ReturnsAsync((Command)null);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(request.ExecutionId));
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.State, Is.EqualTo(ResultState.QueueIsEmpty));
			Assert.That(result.Error, Is.EqualTo("Command cannot be peeked from the queue"));
		}

		[Test]
		public async Task Handle_ExecuteNextCommandFails_ReturnsExecutionError()
		{
			// Arrange
			var request = new ExecuteCommandFromQueueCommand { ExecutionId = Guid.NewGuid() };
			var command = new Command(CommandType.TurnLeft, 1);
			_commandRepositoryMock.Setup(repo => repo.PeekAsync(request.ExecutionId)).ReturnsAsync(command);
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteNextCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = false, Error = "Execution failed", State = ResultState.ExecutionError, ExecutionId = Guid.NewGuid() });

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(request.ExecutionId));
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.State, Is.EqualTo(ResultState.ExecutionError));
			Assert.That(result.Error, Is.EqualTo("Execution failed"));
		}

		[Test]
		public async Task Handle_CommandIsPulledSuccessfully_ReturnsOk()
		{
			// Arrange
			var request = new ExecuteCommandFromQueueCommand { ExecutionId = Guid.NewGuid() };
			var command = new Command(CommandType.TurnLeft, 1) 
			{
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = true,
				IsCompletedByCommand = true,
				IsCompletedByMap = true,
				IsCompletedByRobot = true
			};
			_commandRepositoryMock.Setup(repo => repo.PeekAsync(request.ExecutionId)).ReturnsAsync(command);
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteNextCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = true, State = ResultState.Ok, ExecutionId = Guid.NewGuid() });
			_commandRepositoryMock.Setup(repo => repo.PullAsync(request.ExecutionId)).ReturnsAsync(command);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(request.ExecutionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
		}

		[Test]
		public async Task Handle_CommandIsNotValid_ReturnsError()
		{
			// Arrange
			var request = new ExecuteCommandFromQueueCommand { ExecutionId = Guid.NewGuid() };
			var command = new Command(CommandType.TurnLeft, 1) 
			{
				IsValidatedByCommand = false,
				IsValidatedByMap = false,
				IsValidatedByRobot = false,
				IsCompletedByCommand = false,
				IsCompletedByMap = false,
				IsCompletedByRobot = false
			};
			_commandRepositoryMock.Setup(repo => repo.PeekAsync(request.ExecutionId)).ReturnsAsync(command);
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteNextCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = true, State = ResultState.Ok, ExecutionId = Guid.NewGuid() });
			_commandRepositoryMock.Setup(repo => repo.PullAsync(request.ExecutionId)).ReturnsAsync(command);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(request.ExecutionId));
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.State, Is.EqualTo(ResultState.Error));
			Assert.That(result.Error, Is.EqualTo("Validation of the command failed at the end of operation"));
		}

		[Test]
		public async Task Handle_CommandIsNotCompleted_ReturnsError()
		{
			// Arrange
			var request = new ExecuteCommandFromQueueCommand { ExecutionId = Guid.NewGuid() };
			var command = new Command(CommandType.TurnLeft, 1)
			{
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = true,
				IsCompletedByCommand = false,
				IsCompletedByMap = false,
				IsCompletedByRobot = false
			};
			_commandRepositoryMock.Setup(repo => repo.PeekAsync(request.ExecutionId)).ReturnsAsync(command);
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteNextCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = true, State = ResultState.Ok, ExecutionId = Guid.NewGuid() });
			_commandRepositoryMock.Setup(repo => repo.PullAsync(request.ExecutionId)).ReturnsAsync(command);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(request.ExecutionId));
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.State, Is.EqualTo(ResultState.Error));
			Assert.That(result.Error, Is.EqualTo("Execution of the command failed at the end of operation"));
		}
	}
}
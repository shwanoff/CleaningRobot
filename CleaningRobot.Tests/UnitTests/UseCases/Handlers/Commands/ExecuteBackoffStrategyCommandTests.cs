using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Handlers.Commands;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;
using MediatR;
using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Interfaces;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Commands
{
	[TestFixture]
	public class ExecuteBackoffStrategyCommandTests
	{
		private Mock<IBackoffRepository> _backoffRepositoryMock;
		private Mock<ILogAdapter> _logAdapterMock;
		private Mock<IMediator> _mediatorMock;
		private ExecuteBackoffStrategyCommandHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_backoffRepositoryMock = new Mock<IBackoffRepository>();
			_logAdapterMock = new Mock<ILogAdapter>();
			_mediatorMock = new Mock<IMediator>();
			_handler = new ExecuteBackoffStrategyCommandHandler(_backoffRepositoryMock.Object, _mediatorMock.Object, _logAdapterMock.Object);
		}

		[Test]
		public async Task Handle_BackoffStrategiesNull_ReturnsQueueIsEmpty()
		{
			// Arrange
			var request = new ExecuteBackoffStrategyCommand { ExecutionId = Guid.NewGuid() };
			_backoffRepositoryMock.Setup(repo => repo.PeekAsync(request.ExecutionId)).ReturnsAsync((Queue<Command>)null);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(request.ExecutionId));
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.State, Is.EqualTo(ResultState.QueueIsEmpty));
			Assert.That(result.Error, Is.EqualTo("Backoff strategies cannot be pulled from the queue"));
		}

		[Test]
		public async Task Handle_BackoffStrategiesEmpty_ReturnsQueueIsEmpty()
		{
			// Arrange
			var request = new ExecuteBackoffStrategyCommand { ExecutionId = Guid.NewGuid() };
			_backoffRepositoryMock.Setup(repo => repo.PeekAsync(request.ExecutionId)).ReturnsAsync(new Queue<Command>());

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(request.ExecutionId));
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.State, Is.EqualTo(ResultState.QueueIsEmpty));
			Assert.That(result.Error, Is.EqualTo("Backoff strategies cannot be pulled from the queue"));
		}

		[Test]
		public async Task Handle_ExecuteNextCommandFails_ReturnsExecutionError()
		{
			// Arrange
			var request = new ExecuteBackoffStrategyCommand { ExecutionId = Guid.NewGuid() };
			var backoffCommand = new Command(CommandType.TurnLeft, 1);
			var backoffStrategies = new Queue<Command>(new[] { backoffCommand });

			_backoffRepositoryMock.Setup(repo => repo.PeekAsync(request.ExecutionId)).ReturnsAsync(backoffStrategies);
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
		public async Task Handle_ExecuteNextCommandSucceeds_ReturnsOk()
		{
			// Arrange
			var request = new ExecuteBackoffStrategyCommand { ExecutionId = Guid.NewGuid() };
			var backoffCommand = new Command(CommandType.TurnLeft, 1);
			var backoffStrategies = new Queue<Command>(new[] { backoffCommand });

			_backoffRepositoryMock.Setup(repo => repo.PeekAsync(request.ExecutionId)).ReturnsAsync(backoffStrategies);
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteNextCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = true, State = ResultState.Ok, ExecutionId = Guid.NewGuid() });

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(request.ExecutionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
		}
	}
}
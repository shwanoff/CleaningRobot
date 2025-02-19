using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Handlers.Commands;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;
using MediatR;
using CleaningRobot.UseCases.Dto.Input;
using CleaningRobot.UseCases.Handlers.Maps;
using CleaningRobot.UseCases.Handlers.Robots;
using CleaningRobot.Entities.Interfaces;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Commands
{
	[TestFixture]
	public class StartCommandTests
	{
		private Mock<IMediator> _mediatorMock;
		private Mock<IBackoffRepository> _backoffRepositoryMock;
		private Mock<ILogAdapter> _logAdapterMock;
		private StartCommandHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_mediatorMock = new Mock<IMediator>();
			_backoffRepositoryMock = new Mock<IBackoffRepository>();
			_logAdapterMock = new Mock<ILogAdapter>();
			_handler = new StartCommandHandler(_mediatorMock.Object, _backoffRepositoryMock.Object, _logAdapterMock.Object);
		}

		[Test]
		public async Task Handle_SetupRobotFails_ReturnsError()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var request = new StartCommand { ExecutionId = executionId };

			_mediatorMock.Setup(m => m.Send(It.IsAny<SetupRobotOnMapCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = false, Error = "Setup failed", State = ResultState.Error, ExecutionId = executionId });

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.State, Is.EqualTo(ResultState.Error));
			Assert.That(result.Error, Is.EqualTo("Setup failed"));
		}

		[Test]
		public async Task Handle_ExecuteNextFromQueueFails_ReturnsError()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var request = new StartCommand { ExecutionId = executionId };

			_mediatorMock.Setup(m => m.Send(It.IsAny<SetupRobotOnMapCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = true, State = ResultState.Ok, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteCommandFromQueueCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = false, Error = "Execution failed", State = ResultState.ExecutionError, ExecutionId = executionId });

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.State, Is.EqualTo(ResultState.ExecutionError));
			Assert.That(result.Error, Is.EqualTo("Execution failed"));
		}

		[Test]
		public async Task Handle_ExecuteNextFromQueueOutOfEnergy_ReturnsOk()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var request = new StartCommand { ExecutionId = executionId };

			_mediatorMock.Setup(m => m.Send(It.IsAny<SetupRobotOnMapCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = true, State = ResultState.Ok, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteCommandFromQueueCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = false, State = ResultState.OutOfEnergy, ExecutionId = executionId });

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
		}

		[Test]
		public async Task Handle_ExecuteNextFromQueueBackOff_ReturnsOk()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var request = new StartCommand { ExecutionId = executionId };

			var resultsQueue = new Queue<ResultStatusDto>(new[]
			{
				new ResultStatusDto { IsCorrect = false, State = ResultState.BackOff, ExecutionId = executionId },
				new ResultStatusDto { IsCorrect = false, State = ResultState.QueueIsEmpty, ExecutionId = executionId }
			});

			_mediatorMock.Setup(m => m.Send(It.IsAny<SetupRobotOnMapCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = true, State = ResultState.Ok, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteCommandFromQueueCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(() => resultsQueue.Dequeue());
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteBackoffStrategyCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = true, State = ResultState.Ok, ExecutionId = executionId });

			_backoffRepositoryMock.SetupGet(repo => repo.Settings).Returns(new CommandSettingsDto
			{
				StopWhenBackOff = false,
				ConsumeEnergyWhenBackOff = false
			});

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
		}

		[Test]
		public async Task Handle_ExecuteNextFromQueueBackOffWithStop_ReturnsOk()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var request = new StartCommand { ExecutionId = executionId };

			_mediatorMock.Setup(m => m.Send(It.IsAny<SetupRobotOnMapCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = true, State = ResultState.Ok, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteCommandFromQueueCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = false, State = ResultState.BackOff, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteBackoffStrategyCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = true, State = ResultState.Ok, ExecutionId = executionId });

			_backoffRepositoryMock.SetupGet(repo => repo.Settings).Returns(new CommandSettingsDto
			{
				StopWhenBackOff = true,
				ConsumeEnergyWhenBackOff = false
			});

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
		}

		[Test]
		public async Task Handle_ExecuteNextFromQueueBackOffWithConsumeEnergy_ReturnsOk()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var request = new StartCommand { ExecutionId = executionId };

			var resultsQueue = new Queue<ResultStatusDto>(new[]
			{
				new ResultStatusDto { IsCorrect = false, State = ResultState.BackOff, ExecutionId = executionId },
				new ResultStatusDto { IsCorrect = false, State = ResultState.QueueIsEmpty, ExecutionId = executionId }
			});

			_mediatorMock.Setup(m => m.Send(It.IsAny<SetupRobotOnMapCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = true, State = ResultState.Ok, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteCommandFromQueueCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(() => resultsQueue.Dequeue());
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteBackoffStrategyCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ResultStatusDto { IsCorrect = true, State = ResultState.Ok, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ConsumeEnergyCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ExecutionResultStatusDto<RobotStatusDto> { IsCorrect = true, State = ResultState.Ok, IsCompleted = true, Result = default, ExecutionId = executionId });

			_backoffRepositoryMock.SetupGet(repo => repo.Settings).Returns(new CommandSettingsDto
			{
				StopWhenBackOff = false,
				ConsumeEnergyWhenBackOff = true
			});

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
		}
	}
}
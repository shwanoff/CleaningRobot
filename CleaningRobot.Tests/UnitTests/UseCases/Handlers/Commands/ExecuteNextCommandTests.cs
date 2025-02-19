using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Handlers.Commands;
using Moq;
using MediatR;
using CleaningRobot.UseCases.Handlers.Maps;
using CleaningRobot.UseCases.Handlers.Robots;
using CleaningRobot.Entities.Interfaces;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Commands
{
	[TestFixture]
	public class ExecuteNextCommandTests
	{
		private Mock<IMediator> _mediatorMock;
		private Mock<ILogAdapter> _logAdapterMock;
		private ExecuteNextCommandHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_mediatorMock = new Mock<IMediator>();
			_logAdapterMock = new Mock<ILogAdapter>();
			_handler = new ExecuteNextCommandHandler(_mediatorMock.Object, _logAdapterMock.Object);
		}

		[Test]
		public async Task Handle_ValidRequest_ReturnsOk()
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
			var request = new ExecuteNextCommand
			{
				ExecutionId = executionId,
				Command = command,
				Backoff = false
			};

			_mediatorMock.Setup(m => m.Send(It.IsAny<ValidateCommandByCommandQuery>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResultStatusDto { IsCorrect = true, IsValid = true, State = ResultState.Ok, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ValidateCommandByMapQuery>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResultStatusDto { IsCorrect = true, IsValid = true, State = ResultState.Ok, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ValidateCommandByRobotQuery>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResultStatusDto { IsCorrect = true, IsValid = true, State = ResultState.Ok, ExecutionId = executionId	 });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteCommandCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ExecutionResultStatusDto<CommandStatusDto> { IsCorrect = true, State = ResultState.Ok, IsCompleted = true, Result = default, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteMapCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ExecutionResultStatusDto<MapStatusDto> { IsCorrect = true, State = ResultState.Ok, IsCompleted = true, Result = default, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteRobotCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ExecutionResultStatusDto<RobotStatusDto> { IsCorrect = true, State = ResultState.Ok, IsCompleted = true, Result = default, ExecutionId = executionId });

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
		}

		[Test]
		public async Task Handle_InvalidCommandValidation_ReturnsValidationError()
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
			var request = new ExecuteNextCommand
			{
				ExecutionId = executionId,
				Command = command,
				Backoff = false
			};

			_mediatorMock.Setup(m => m.Send(It.IsAny<ValidateCommandByCommandQuery>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResultStatusDto { IsCorrect = false, IsValid = false, State = ResultState.ValidationError, Error = "Invalid command", ExecutionId = executionId });

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.State, Is.EqualTo(ResultState.ValidationError));
			Assert.That(result.Error, Is.EqualTo("Invalid command"));
		}

		[Test]
		public async Task Handle_InvalidMapValidation_ReturnsValidationError()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.TurnLeft, 1)
			{
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = true,
				IsCompletedByCommand = true,
				IsCompletedByMap = false,
				IsCompletedByRobot = true
			}; ;
			var request = new ExecuteNextCommand
			{
				ExecutionId = executionId,
				Command = command,
				Backoff = false
			};

			_mediatorMock.Setup(m => m.Send(It.IsAny<ValidateCommandByCommandQuery>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResultStatusDto { IsCorrect = true, IsValid = true, State = ResultState.Ok, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ValidateCommandByMapQuery>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResultStatusDto { IsCorrect = false, IsValid = false, State = ResultState.ValidationError, Error = "Invalid map", ExecutionId = executionId });

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.State, Is.EqualTo(ResultState.ValidationError));
			Assert.That(result.Error, Is.EqualTo("Invalid map"));
		}

		[Test]
		public async Task Handle_InvalidRobotValidation_ReturnsValidationError()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.TurnLeft, 1)
			{
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = false,
				IsCompletedByCommand = true,
				IsCompletedByMap = true,
				IsCompletedByRobot = true
			};
			var request = new ExecuteNextCommand
			{
				ExecutionId = executionId,
				Command = command,
				Backoff = false
			};

			_mediatorMock.Setup(m => m.Send(It.IsAny<ValidateCommandByCommandQuery>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResultStatusDto { IsCorrect = true, IsValid = true, State = ResultState.Ok, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ValidateCommandByMapQuery>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResultStatusDto { IsCorrect = true, IsValid = true, State = ResultState.Ok, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ValidateCommandByRobotQuery>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResultStatusDto { IsCorrect = false, IsValid = false, State = ResultState.ValidationError, Error = "Invalid robot", ExecutionId = executionId });

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.State, Is.EqualTo(ResultState.ValidationError));
			Assert.That(result.Error, Is.EqualTo("Invalid robot"));
		}

		[Test]
		public async Task Handle_ExecutionFails_ReturnsExecutionError()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.TurnLeft, 1)
			{
				IsValidatedByCommand = true,
				IsValidatedByMap = true,
				IsValidatedByRobot = true,
				IsCompletedByCommand = false,
				IsCompletedByMap = false,
				IsCompletedByRobot = false
			};
			var request = new ExecuteNextCommand
			{
				ExecutionId = executionId,
				Command = command,
				Backoff = false
			};

			_mediatorMock.Setup(m => m.Send(It.IsAny<ValidateCommandByCommandQuery>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResultStatusDto { IsCorrect = true, IsValid = true, State = ResultState.Ok, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ValidateCommandByMapQuery>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResultStatusDto { IsCorrect = true, IsValid = true, State = ResultState.Ok, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ValidateCommandByRobotQuery>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResultStatusDto { IsCorrect = true, IsValid = true, State = ResultState.Ok, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteCommandCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ExecutionResultStatusDto<CommandStatusDto> { IsCorrect = false, State = ResultState.ExecutionError, Error = "Execution failed", IsCompleted = false, Result = default, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteMapCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ExecutionResultStatusDto<MapStatusDto> { IsCorrect = true, State = ResultState.Ok, IsCompleted = true, Result = default, ExecutionId = executionId });
			_mediatorMock.Setup(m => m.Send(It.IsAny<ExecuteRobotCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ExecutionResultStatusDto<RobotStatusDto> { IsCorrect = true, State = ResultState.Ok, IsCompleted = true, Result = default, ExecutionId = executionId });



			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.State, Is.EqualTo(ResultState.ExecutionError));
			Assert.That(result.Error, Is.EqualTo("Execution failed"));
		}
	}
}
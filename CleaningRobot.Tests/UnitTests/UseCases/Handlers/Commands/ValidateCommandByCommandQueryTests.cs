using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Handlers.Commands;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Commands
{
	[TestFixture]
	public class ValidateCommandByCommandQueryTests
	{
		private Mock<IQueueRepository<Command>> _commandRepositoryMock;
		private Mock<IBackoffRepository> _backoffRepositoryMock;
		private Mock<ILogAdapter> _logAdapterMock;
		private ValidateCommandByCommandQueryHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_commandRepositoryMock = new Mock<IQueueRepository<Command>>();
			_backoffRepositoryMock = new Mock<IBackoffRepository>();
			_logAdapterMock = new Mock<ILogAdapter>();
			_handler = new ValidateCommandByCommandQueryHandler(_commandRepositoryMock.Object, _backoffRepositoryMock.Object, _logAdapterMock.Object);
		}

		[Test]
		public async Task Handle_NullRequest_ReturnsValidationError()
		{
			// Arrange
			ValidateCommandByCommandQuery request = null;

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Error, Is.EqualTo("Request cannot be null"));
			Assert.That(result.State, Is.EqualTo(ResultState.ValidationError));
		}

		[Test]
		public async Task Handle_NullCommand_ReturnsValidationError()
		{
			// Arrange
			var request = new ValidateCommandByCommandQuery
			{
				ExecutionId = Guid.NewGuid(),
				Command = null,
				Backoff = false
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
			var command = new Command(CommandType.TurnLeft, -1);
			var request = new ValidateCommandByCommandQuery
			{
				ExecutionId = Guid.NewGuid(),
				Command = command,
				Backoff = false
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
		public async Task Handle_ValidCommand_UpdatesCommandAndReturnsOk()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.TurnLeft, 1);
			var request = new ValidateCommandByCommandQuery
			{
				ExecutionId = executionId,
				Command = command,
				Backoff = false
			};

			_commandRepositoryMock.Setup(repo => repo.UpdateFirstAsync(command, executionId))
				.ReturnsAsync(command);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.IsValid, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
		}

		[Test]
		public async Task Handle_ValidBackoffCommand_UpdatesCommandAndReturnsOk()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.TurnLeft, 1);
			var request = new ValidateCommandByCommandQuery
			{
				ExecutionId = executionId,
				Command = command,
				Backoff = true
			};

			_backoffRepositoryMock.Setup(repo => repo.UpdateFirstAsync(command, executionId))
				.ReturnsAsync(command);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.IsValid, Is.True);
			Assert.That(result.State, Is.EqualTo(ResultState.Ok));
		}

		[Test]
		public async Task Handle_UpdateCommandFails_ReturnsValidationError()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var command = new Command(CommandType.TurnLeft, 1);
			var request = new ValidateCommandByCommandQuery
			{
				ExecutionId = executionId,
				Command = command,
				Backoff = false
			};

			_commandRepositoryMock.Setup(repo => repo.UpdateFirstAsync(command, executionId))
				.ReturnsAsync((Command)null);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.IsCorrect, Is.False);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Error, Is.EqualTo("Cannot update the command after validation"));
			Assert.That(result.State, Is.EqualTo(ResultState.ValidationError));
		}
	}
}
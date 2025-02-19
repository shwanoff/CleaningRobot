using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Handlers.Commands;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Commands
{
	[TestFixture]
	public class GetCommandQueueQueryTests
	{
		private Mock<IRepository<Queue<Command>>> _commandRepositoryMock;
		private Mock<ILogAdapter> _logAdapterMock;
		private GetCommandQueueQueryHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_commandRepositoryMock = new Mock<IRepository<Queue<Command>>>();
			_logAdapterMock = new Mock<ILogAdapter>();
			_handler = new GetCommandQueueQueryHandler(_commandRepositoryMock.Object, _logAdapterMock.Object);
		}

		[Test]
		public async Task Handle_ValidRequest_ReturnsCorrectStatus()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var commandQueue = new Queue<Command>(new[]
			{
				new Command(CommandType.TurnLeft, 1)
				{
					IsValidatedByCommand = true,
					IsValidatedByMap = true,
					IsValidatedByRobot = true,
					IsCompletedByCommand = true,
					IsCompletedByMap = true,
					IsCompletedByRobot = true
				},
				new Command(CommandType.Advance, 2)
				{
					IsValidatedByCommand = true,
					IsValidatedByMap = true,
					IsValidatedByRobot = true,
					IsCompletedByCommand = false,
					IsCompletedByMap = false,
					IsCompletedByRobot = false
				}
			});

			_commandRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(commandQueue);

			var request = new GetCommandQueueQuery
			{
				ExecutionId = executionId
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.Commands.Count, Is.EqualTo(2));
			Assert.That(result.Commands[0].Type, Is.EqualTo(CommandType.TurnLeft));
			Assert.That(result.Commands[0].EnergyConsumption, Is.EqualTo(1));
			Assert.That(result.Commands[0].IsValid, Is.True);
			Assert.That(result.Commands[0].IsCompleted, Is.True);
			Assert.That(result.Commands[1].Type, Is.EqualTo(CommandType.Advance));
			Assert.That(result.Commands[1].EnergyConsumption, Is.EqualTo(2));
			Assert.That(result.Commands[1].IsValid, Is.True);
			Assert.That(result.Commands[1].IsCompleted, Is.False);
		}

		[Test]
		public void Handle_NullRequest_ThrowsArgumentNullException()
		{
			// Arrange
			GetCommandQueueQuery request = null;

			// Act & Assert
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(request, CancellationToken.None));
		}

		[Test]
		public void Handle_CommandQueueNotFound_ThrowsArgumentNullException()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			_commandRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync((Queue<Command>)null);

			var request = new GetCommandQueueQuery
			{
				ExecutionId = executionId
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(request, CancellationToken.None));
		}
	}
}
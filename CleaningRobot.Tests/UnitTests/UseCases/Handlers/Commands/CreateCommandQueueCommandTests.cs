using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Handlers.Commands;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Commands
{
	[TestFixture]
	public class CreateCommandQueueCommandTests
	{
		private Mock<IRepository<Queue<Command>>> _commandRepositoryMock;
		private Mock<ILogAdapter> _logAdapterMock;
		private CreateCommandQueueCommandHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_commandRepositoryMock = new Mock<IRepository<Queue<Command>>>();
			_logAdapterMock = new Mock<ILogAdapter>();
			_handler = new CreateCommandQueueCommandHandler(_commandRepositoryMock.Object, _logAdapterMock.Object);
		}

		[Test]
		public async Task Handle_ValidRequest_ReturnsCorrectStatus()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var commands = new List<CommandType> { CommandType.TurnLeft, CommandType.Advance };
			var energyConsumptions = new Dictionary<CommandType, int>
			{
				{ CommandType.TurnLeft, 1 },
				{ CommandType.Advance, 2 }
			};

			var request = new CreateCommandQueueCommand
			{
				ExecutionId = executionId,
				Commands = commands,
				EnergyConsumptions = energyConsumptions
			};

			_commandRepositoryMock
				.Setup(repo => repo.AddAsync(It.IsAny<Queue<Command>>(), executionId))
				.ReturnsAsync(new Queue<Command>(new[]
				{
					new Command(CommandType.TurnLeft, 1),
					new Command(CommandType.Advance, 2)
				}));

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.Commands.Count, Is.EqualTo(2));
			Assert.That(result.Commands[0].Type, Is.EqualTo(CommandType.TurnLeft));
			Assert.That(result.Commands[0].EnergyConsumption, Is.EqualTo(1));
			Assert.That(result.Commands[1].Type, Is.EqualTo(CommandType.Advance));
			Assert.That(result.Commands[1].EnergyConsumption, Is.EqualTo(2));
		}

		[Test]
		public void Handle_NullRequest_ThrowsArgumentNullException()
		{
			// Arrange
			CreateCommandQueueCommand request = null;

			// Act & Assert
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(request, CancellationToken.None));
		}

		[Test]
		public void Handle_NullCommands_ThrowsArgumentNullException()
		{
			// Arrange
			var request = new CreateCommandQueueCommand
			{
				ExecutionId = Guid.NewGuid(),
				Commands = null,
				EnergyConsumptions = new Dictionary<CommandType, int>()
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(request, CancellationToken.None));
		}

		[Test]
		public void Handle_NullEnergyConsumptions_ThrowsArgumentNullException()
		{
			// Arrange
			var request = new CreateCommandQueueCommand
			{
				ExecutionId = Guid.NewGuid(),
				Commands = new List<CommandType>(),
				EnergyConsumptions = null
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(request, CancellationToken.None));
		}
	}
}

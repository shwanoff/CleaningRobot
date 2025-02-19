using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Input;
using CleaningRobot.UseCases.Handlers.Commands;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Commands
{
	[TestFixture]
	public class SetupBackoffStrategyCommandTests
	{
		private Mock<IBackoffRepository> _backoffRepositoryMock;
		private SetupBackoffStrategyCommandHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_backoffRepositoryMock = new Mock<IBackoffRepository>();
			_handler = new SetupBackoffStrategyCommandHandler(_backoffRepositoryMock.Object);
		}

		[Test]
		public async Task Handle_ValidRequest_ReturnsCorrectStatus()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var backoffCommands = new List<List<CommandType>>
			{
				new List<CommandType> { CommandType.TurnLeft, CommandType.Advance },
				new List<CommandType> { CommandType.TurnRight, CommandType.Back }
			};
			var energyConsumptions = new Dictionary<CommandType, int>
			{
				{ CommandType.TurnLeft, 1 },
				{ CommandType.Advance, 2 },
				{ CommandType.TurnRight, 1 },
				{ CommandType.Back, 3 }
			};
			var commandSettings = new CommandSettingsDto
			{
				StopWhenBackOff = true,
				ConsumeEnergyWhenBackOff = false
			};

			var request = new SetupBackoffStrategyCommand
			{
				ExecutionId = executionId,
				BackoffCommands = backoffCommands,
				EnergyConsumptions = energyConsumptions,
				CommandSettings = commandSettings
			};

			var resultQueueOfQueues = new Queue<Queue<Command>>(new[]
			{
				new Queue<Command>(new[]
				{
					new Command(CommandType.TurnLeft, 1),
					new Command(CommandType.Advance, 2)
				}),
				new Queue<Command>(new[]
				{
					new Command(CommandType.TurnRight, 1),
					new Command(CommandType.Back, 3)
				})
			});

			_backoffRepositoryMock.Setup(repo => repo.Initialize(It.IsAny<IEnumerable<IEnumerable<Command>>>(), executionId))
				.ReturnsAsync(resultQueueOfQueues);

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.Commands.Count, Is.EqualTo(4));
			Assert.That(result.Commands[0].Type, Is.EqualTo(CommandType.TurnLeft));
			Assert.That(result.Commands[0].EnergyConsumption, Is.EqualTo(1));
			Assert.That(result.Commands[1].Type, Is.EqualTo(CommandType.Advance));
			Assert.That(result.Commands[1].EnergyConsumption, Is.EqualTo(2));
			Assert.That(result.Commands[2].Type, Is.EqualTo(CommandType.TurnRight));
			Assert.That(result.Commands[2].EnergyConsumption, Is.EqualTo(1));
			Assert.That(result.Commands[3].Type, Is.EqualTo(CommandType.Back));
			Assert.That(result.Commands[3].EnergyConsumption, Is.EqualTo(3));
		}

		[Test]
		public void Handle_NullRequest_ThrowsArgumentNullException()
		{
			// Arrange
			SetupBackoffStrategyCommand request = null;

			// Act & Assert
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(request, CancellationToken.None));
		}

		[Test]
		public void Handle_EmptyBackoffCommands_ThrowsArgumentException()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var energyConsumptions = new Dictionary<CommandType, int>
			{
				{ CommandType.TurnLeft, 1 },
				{ CommandType.Advance, 2 }
			};
			var commandSettings = new CommandSettingsDto
			{
				StopWhenBackOff = true,
				ConsumeEnergyWhenBackOff = false
			};

			var request = new SetupBackoffStrategyCommand
			{
				ExecutionId = executionId,
				BackoffCommands = new List<List<CommandType>>(),
				EnergyConsumptions = energyConsumptions,
				CommandSettings = commandSettings
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(request, CancellationToken.None));
		}

		[Test]
		public void Handle_EmptyEnergyConsumptions_ThrowsArgumentException()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var backoffCommands = new List<List<CommandType>>
			{
				new List<CommandType> { CommandType.TurnLeft, CommandType.Advance }
			};
			var commandSettings = new CommandSettingsDto
			{
				StopWhenBackOff = true,
				ConsumeEnergyWhenBackOff = false
			};

			var request = new SetupBackoffStrategyCommand
			{
				ExecutionId = executionId,
				BackoffCommands = backoffCommands,
				EnergyConsumptions = new Dictionary<CommandType, int>(),
				CommandSettings = commandSettings
			};

			// Act & Assert
			Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(request, CancellationToken.None));
		}
	}
}
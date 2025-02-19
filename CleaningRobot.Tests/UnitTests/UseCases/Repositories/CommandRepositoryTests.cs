using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Repositories;

namespace CleaningRobot.Tests.UnitTests.UseCases.Repositories
{
	[TestFixture]
	public class CommandRepositoryTests
	{
		private CommandRepository _repository;

		[SetUp]
		public void SetUp()
		{
			_repository = new CommandRepository();
		}

		[Test]
		public async Task AddAsync_AddsNewCommand()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var commandQueue = new Queue<Command>();

			// Act
			var result = await _repository.AddAsync(commandQueue, executionId);

			// Assert
			Assert.That(result, Is.EqualTo(commandQueue));
		}

		[Test]
		public async Task DeleteAsync_RemovesCommand()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var commandQueue = new Queue<Command>();
			await _repository.AddAsync(commandQueue, executionId);

			// Act
			await _repository.DeleteAsync(executionId);

			// Assert
			Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.GetByIdAsync(executionId));
		}

		[Test]
		public async Task GetAllAsync_ReturnsAllCommands()
		{
			// Arrange
			var executionId1 = Guid.NewGuid();
			var executionId2 = Guid.NewGuid();
			var commandQueue1 = new Queue<Command>();
			var commandQueue2 = new Queue<Command>();
			await _repository.AddAsync(commandQueue1, executionId1);
			await _repository.AddAsync(commandQueue2, executionId2);

			// Act
			var result = await _repository.GetAllAsync();

			// Assert
			Assert.That(result, Has.Exactly(2).Items);
		}

		[Test]
		public async Task GetByIdAsync_ReturnsCommandById()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var commandQueue = new Queue<Command>();
			await _repository.AddAsync(commandQueue, executionId);

			// Act
			var result = await _repository.GetByIdAsync(executionId);

			// Assert
			Assert.That(result, Is.EqualTo(commandQueue));
		}

		[Test]
		public async Task PeekAsync_ReturnsFirstCommandWithoutRemoving()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var commandQueue = new Queue<Command>();
			var command = new Command(CommandType.Clean, 10);
			commandQueue.Enqueue(command);
			await _repository.AddAsync(commandQueue, executionId);

			// Act
			var result = await _repository.PeekAsync(executionId);

			// Assert
			Assert.That(result, Is.EqualTo(command));
		}

		[Test]
		public async Task PullAsync_ReturnsAndRemovesFirstCommand()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var commandQueue = new Queue<Command>();
			var command = new Command(CommandType.Clean, 10);
			commandQueue.Enqueue(command);
			await _repository.AddAsync(commandQueue, executionId);

			// Act
			var result = await _repository.PullAsync(executionId);

			// Assert
			Assert.That(result, Is.EqualTo(command));
			Assert.That(commandQueue.Count, Is.EqualTo(0));
		}

		[Test]
		public async Task PushAsync_AddsNewCommandToQueue()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var commandQueue = new Queue<Command>();
			await _repository.AddAsync(commandQueue, executionId);
			var command = new Command(CommandType.Clean, 10);

			// Act
			await _repository.PushAsync(command, executionId);

			// Assert
			Assert.That(commandQueue.Count, Is.EqualTo(1));
		}

		[Test]
		public async Task UpdateFirstAsync_UpdatesFirstCommandInQueue()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var commandQueue = new Queue<Command>();
			var command = new Command(CommandType.Clean, 10);
			commandQueue.Enqueue(command);
			await _repository.AddAsync(commandQueue, executionId);
			var newCommand = new Command(CommandType.Advance, 20) { IsValidatedByCommand = true };

			// Act
			var result = await _repository.UpdateFirstAsync(newCommand, executionId);

			// Assert
			Assert.That(result.IsValidatedByCommand, Is.EqualTo(newCommand.IsValidatedByCommand));
			Assert.That(result.EnergyConsumption, Is.EqualTo(command.EnergyConsumption));
			Assert.That(result.Type, Is.EqualTo(command.Type));
			Assert.That(result.EnergyConsumption, Is.Not.EqualTo(newCommand.EnergyConsumption));
			Assert.That(result.Type, Is.Not.EqualTo(newCommand.Type));

		}
	}
}
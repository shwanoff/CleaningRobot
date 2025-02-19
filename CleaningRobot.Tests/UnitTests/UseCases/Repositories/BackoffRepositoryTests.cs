using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Input;
using CleaningRobot.UseCases.Repositories;

namespace CleaningRobot.Tests.UnitTests.UseCases.Repositories
{
	[TestFixture]
	public class BackoffRepositoryTests
	{
		private BackoffRepository _repository;

		[SetUp]
		public void SetUp()
		{
			_repository = new BackoffRepository
			{
				Settings = new CommandSettingsDto
				{
					ConsumeEnergyWhenBackOff = true,
					StopWhenBackOff = true
				}
			};
		}

		[Test]
		public async Task AddAsync_AddsNewBackoffStrategy()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var queue = new Queue<Queue<Command>>();

			// Act
			var result = await _repository.AddAsync(queue, executionId);

			// Assert
			Assert.That(result, Is.EqualTo(queue));
		}

		[Test]
		public async Task DeleteAsync_RemovesBackoffStrategy()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var queue = new Queue<Queue<Command>>();
			await _repository.AddAsync(queue, executionId);

			// Act
			await _repository.DeleteAsync(executionId);

			// Assert
			Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.GetByIdAsync(executionId));
		}

		[Test]
		public async Task GetAllAsync_ReturnsAllBackoffStrategies()
		{
			// Arrange
			var executionId1 = Guid.NewGuid();
			var executionId2 = Guid.NewGuid();
			var queue1 = new Queue<Queue<Command>>();
			var queue2 = new Queue<Queue<Command>>();
			await _repository.AddAsync(queue1, executionId1);
			await _repository.AddAsync(queue2, executionId2);

			// Act
			var result = await _repository.GetAllAsync();

			// Assert
			Assert.That(result, Has.Exactly(2).Items);
		}

		[Test]
		public async Task GetByIdAsync_ReturnsBackoffStrategyById()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var queue = new Queue<Queue<Command>>();
			await _repository.AddAsync(queue, executionId);

			// Act
			var result = await _repository.GetByIdAsync(executionId);

			// Assert
			Assert.That(result, Is.EqualTo(queue));
		}

		[Test]
		public async Task Initialize_InitializesBackoffStrategy()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var commands = new List<List<Command>>
			{
				new List<Command> { new Command(CommandType.Clean, 10) },
				new List<Command> { new Command(CommandType.Advance, 20) }
			};

			// Act
			var result = await _repository.Initialize(commands, executionId);

			// Assert
			Assert.That(result.Count, Is.EqualTo(2));
		}

		[Test]
		public async Task PeekAsync_ReturnsFirstQueueWithoutRemoving()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var queue = new Queue<Queue<Command>>();
			var commandQueue = new Queue<Command>();
			queue.Enqueue(commandQueue);
			await _repository.AddAsync(queue, executionId);

			// Act
			var result = await _repository.PeekAsync(executionId);

			// Assert
			Assert.That(result, Is.EqualTo(commandQueue));
		}

		[Test]
		public async Task PullAsync_ReturnsAndRemovesFirstQueue()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var queue = new Queue<Queue<Command>>();
			var commandQueue = new Queue<Command>();
			queue.Enqueue(commandQueue);
			await _repository.AddAsync(queue, executionId);

			// Act
			var result = await _repository.PullAsync(executionId);

			// Assert
			Assert.That(result, Is.EqualTo(commandQueue));
			Assert.That(queue.Count, Is.EqualTo(0));
		}

		[Test]
		public async Task PushAsync_AddsNewQueueToBackoffStrategy()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var queue = new Queue<Queue<Command>>();
			await _repository.AddAsync(queue, executionId);
			var commandQueue = new Queue<Command>();

			// Act
			await _repository.PushAsync(commandQueue, executionId);

			// Assert
			Assert.That(queue.Count, Is.EqualTo(1));
		}

		[Test]
		public async Task UpdateFirstAsync_UpdatesFirstQueueInBackoffStrategy()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var queue = new Queue<Queue<Command>>();
			var commandQueue = new Queue<Command>();
			queue.Enqueue(commandQueue);
			await _repository.AddAsync(queue, executionId);
			var newCommandQueue = new Queue<Command>();
			newCommandQueue.Enqueue(new Command(CommandType.Clean, 10));

			// Act
			var result = await _repository.UpdateFirstAsync(newCommandQueue, executionId);

			// Assert
			Assert.That(result, Is.EqualTo(newCommandQueue));
		}
	}
}
using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Input;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;

namespace CleaningRobot.UseCases.Repositories
{
	public class BackoffRepository : IBackoffRepository
	{
		private readonly List<List<Command>> _initialBackoffStrategies = new();
		private readonly Dictionary<Guid, Queue<Queue<Command>>> _backoffStrategies = new();

		public required CommandSettingsDto Settings { get; set; } = new()
		{ 
			ConsumeEnergyWhenBackOff = true,
			StopWhenBackOff = true
		};

		public Task<Queue<Queue<Command>>> AddAsync(Queue<Queue<Command>> entity, Guid executionId)
		{
			entity.NotNull();
			_backoffStrategies.KeyNotExists(executionId);

			_backoffStrategies[executionId] = entity;
			var result = _backoffStrategies[executionId];

			return Task.FromResult(result);
		}

		public Task DeleteAsync(Guid executionId)
		{
			_backoffStrategies.KeyExists(executionId);

			_backoffStrategies.Remove(executionId);
			return Task.CompletedTask;
		}

		public Task<IEnumerable<Queue<Queue<Command>>>> GetAllAsync()
		{
			return Task.FromResult<IEnumerable<Queue<Queue<Command>>>>(_backoffStrategies.Values);
		}

		public Task<Queue<Queue<Command>>> GetByIdAsync(Guid executionId)
		{
			_backoffStrategies.KeyExists(executionId);

			var result = _backoffStrategies[executionId];
			return Task.FromResult(result);
		}

		public Task<Queue<Queue<Command>>> Initialize(IEnumerable<IEnumerable<Command>> entity, Guid executionId)
		{
			entity.NotNull();
			entity.HasItems();

			SetupFullBackoffStrategy(entity, _initialBackoffStrategies);

			RefreshBackoffStrategy(executionId);

			var result = _backoffStrategies[executionId];
			return Task.FromResult(result);
		}

		public Task<Queue<Command>?> PeekAsync(Guid executionId)
		{
			_backoffStrategies.KeyExists(executionId);

			var result = _backoffStrategies[executionId];

			if (result.Count == 0)
			{
				return Task.FromResult<Queue<Command>?>(null);
			}
			else
			{
				return Task.FromResult<Queue<Command>?>(result.Peek());
			}
		}

		public Task<Queue<Command>?> PullAsync(Guid executionId)
		{
			_backoffStrategies.KeyExists(executionId);

			var result = _backoffStrategies[executionId];

			if (result.Count == 0)
			{
				return Task.FromResult<Queue<Command>?>(null);
			}
			else
			{
				return Task.FromResult<Queue<Command>?>(result.Dequeue());
			}
		}

		public Task PushAsync(Queue<Command> entity, Guid executionId)
		{
			_backoffStrategies.KeyExists(executionId);

			var result = _backoffStrategies[executionId];
			result.Enqueue(entity);

			return Task.CompletedTask;
		}

		public Task<IEnumerable<Queue<Command>>> ReadAllAsync(Guid executionId)
		{
			_backoffStrategies.KeyExists(executionId);

			var result = _backoffStrategies[executionId];
			return Task.FromResult<IEnumerable<Queue<Command>>>(result);
		}

		public Task<Queue<Queue<Command>>> Refresh(Guid executionId)
		{
			RefreshBackoffStrategy(executionId);

			var result = _backoffStrategies[executionId];
			return Task.FromResult(result);
		}

		public Task<Queue<Queue<Command>>> UpdateAsync(Queue<Queue<Command>> entity, Guid executionId)
		{
			entity.NotNull();
			_backoffStrategies.KeyExists(executionId);

			_backoffStrategies[executionId] = entity;
			var result = _backoffStrategies[executionId];

			return Task.FromResult(result);
		}

		public Task<Queue<Command>> UpdateFirstAsync(Queue<Command> entity, Guid executionId)
		{
			entity.NotNull();
			_backoffStrategies.KeyExists(executionId);

			var current = _backoffStrategies[executionId].Peek();
			current.Clear();

			foreach (var command in entity)
			{
				current.Enqueue(command);
			}

			var result = _backoffStrategies[executionId].Peek();
			return Task.FromResult(result);
		}

		private static void SetupFullBackoffStrategy(IEnumerable<IEnumerable<Command>> newEntity, List<List<Command>> original)
		{
			original.Clear();

			foreach (var newQueue in newEntity)
			{
				var resultQueue = new List<Command>();
				foreach (var newCommand in newQueue)
				{
					resultQueue.Add(new Command(newCommand.Type, newCommand.EnergyConsumption));
				}
				original.Add(resultQueue);
			}
		}

		private void RefreshBackoffStrategy(Guid executionId)
		{
			var result = new Queue<Queue<Command>>();
			foreach (var queue in _initialBackoffStrategies)
			{
				var resultQueue = new Queue<Command>();
				foreach (var command in queue)
				{
					resultQueue.Enqueue(new Command(command.Type, command.EnergyConsumption));
				}
				result.Enqueue(resultQueue);
			}
			_backoffStrategies[executionId] = result;
		}

		public Task<Command> UpdateFirstAsync(Command entity, Guid executionId)
		{
			entity.NotNull();
			_backoffStrategies.KeyExists(executionId);

			var currentElement = _backoffStrategies[executionId].Peek().Peek();

			UpdateItem(currentElement, entity);

			var result = _backoffStrategies[executionId].Peek().Peek();

			return Task.FromResult(result);
		}

		private static void UpdateItem(Command currentElement, Command entity)
		{
			currentElement.IsValidatedByMap = entity.IsValidatedByMap;
			currentElement.IsValidatedByRobot = entity.IsValidatedByRobot;
			currentElement.IsValidatedByCommand = entity.IsValidatedByCommand;
			currentElement.IsCompletedByMap = entity.IsCompletedByMap;
			currentElement.IsCompletedByRobot = entity.IsCompletedByRobot;
			currentElement.IsCompletedByCommand = entity.IsCompletedByCommand;
		}
	}
}

using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces;

namespace CleaningRobot.UseCases.Repositories
{
	public class CommandRepository : IQueueRepository<Command>, IRepository<Queue<Command>>
	{
		private readonly Dictionary<Guid, Queue<Command>> _commands = new();
		public Task<Queue<Command>> AddAsync(Queue<Command> entity, Guid executionId)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity), "Command queue entity is not provided.");
			}

			if (_commands.ContainsKey(executionId))
			{
				throw new InvalidOperationException("Commands for this execution ID already exist.");
			}

			_commands[executionId] = entity;
			var result = _commands[executionId];

			return Task.FromResult(result);
		}

		public Task DeleteAsync(Guid executionId)
		{
			if (!_commands.ContainsKey(executionId))
			{
				throw new KeyNotFoundException("Commands for this execution ID do not exist.");
			}

			_commands.Remove(executionId);
			return Task.CompletedTask;
		}

		public Task<IEnumerable<Queue<Command>>> GetAllAsync()
		{
			return Task.FromResult<IEnumerable<Queue<Command>>>(_commands.Values);
		}

		public Task<Queue<Command>> GetByIdAsync(Guid executionId)
		{
			if (!_commands.TryGetValue(executionId, out Queue<Command>? value))
			{
				throw new KeyNotFoundException("Commands for this execution ID do not exist.");
			}

			return Task.FromResult(value);
		}

		public Task<IEnumerable<Command>> ReadAllAsync(Guid executionId)
		{
			if (!_commands.TryGetValue(executionId, out Queue<Command>? value))
			{
				throw new KeyNotFoundException("Commands for this execution ID do not exist.");
			}

			return Task.FromResult<IEnumerable<Command>>(value);
		}

		public Task<Command?> PeekAsync(Guid executionId)
		{
			if (!_commands.TryGetValue(executionId, out Queue<Command>? value))
			{
				throw new KeyNotFoundException("Commands for this execution ID do not exist.");
			}

			if (value.Count == 0)
			{
				return Task.FromResult<Command?>(null);
			}
			else 
			{ 
				return Task.FromResult(value.Peek());
			}
		}

		public Task<Command?> PullAsync(Guid executionId)
		{
			if (!_commands.TryGetValue(executionId, out Queue<Command>? value))
			{
				throw new KeyNotFoundException("Commands for this execution ID do not exist.");
			}

			if (value.Count == 0)
			{
				return Task.FromResult<Command?>(null);
			}

			return Task.FromResult<Command?>(value.Dequeue());
		}

		public Task PushAsync(Command entity, Guid executionId)
		{
			if (!_commands.TryGetValue(executionId, out Queue<Command>? value))
			{
				throw new KeyNotFoundException("Commands for this execution ID do not exist.");
			}

			value.Enqueue(entity);
			return Task.CompletedTask;
		}

		public Task<Queue<Command>> UpdateAsync(Queue<Command> entity, Guid executionId)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity), "Entity is not provided.");
			}

			if (!_commands.TryGetValue(executionId, out Queue<Command>? queue))
			{
				throw new KeyNotFoundException("Commands for this execution ID do not exist.");
			}

			_commands[executionId] = entity;
			var result = _commands[executionId];

			return Task.FromResult(result);
		}

		public Task<Command> UpdateFirstAsync(Dictionary<string, object> valuesToUpdate, Guid executionId)
		{
			if (valuesToUpdate == null)
			{
				throw new ArgumentNullException(nameof(valuesToUpdate), "Values to update are not provided.");
			}

			if (valuesToUpdate.Count == 0)
			{
				throw new ArgumentException("Values to update are not provided.");
			}

			if (!_commands.TryGetValue(executionId, out Queue<Command>? value))
			{
				throw new KeyNotFoundException("Commands for this execution ID do not exist.");
			}

			if (value.Count == 0)
			{
				throw new InvalidOperationException("The command queue is empty.");
			}

			var currentElement = value.Peek();

			RepositoryHelper.UpdateItem(currentElement, valuesToUpdate);
			
			var resultQueue = _commands[executionId];
			var result = resultQueue.Peek();

			return Task.FromResult(result);
		}
	}
}

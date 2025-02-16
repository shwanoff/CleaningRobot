using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Interfaces;

namespace CleaningRobot.UseCases.Repositories
{
	public class CommandRepository : IQueueRepository<Command>
	{
		private readonly Dictionary<Guid, Queue<Command>> _commands = [];
		public Task AddAsync(Guid executionId, Queue<Command> entity)
		{
			if (_commands.ContainsKey(executionId))
			{
				throw new InvalidOperationException("Commands for this execution ID already exist.");
			}

			_commands[executionId] = entity;
			return Task.CompletedTask;
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

		public Task<IEnumerable<Command>> PeekAllAsync(Guid executionId)
		{
			if (!_commands.TryGetValue(executionId, out Queue<Command>? value))
			{
				throw new KeyNotFoundException("Commands for this execution ID do not exist.");
			}

			return Task.FromResult<IEnumerable<Command>>(value);
		}

		public Task<Command> PeekFirstAsync(Guid executionId)
		{
			if (!_commands.TryGetValue(executionId, out Queue<Command>? value))
			{
				throw new KeyNotFoundException("Commands for this execution ID do not exist.");
			}

			return Task.FromResult(value.Peek());
		}

		public Task<Command> PullAsync(Guid executionId)
		{
			if (!_commands.TryGetValue(executionId, out Queue<Command>? value))
			{
				throw new KeyNotFoundException("Commands for this execution ID do not exist.");
			}

			return Task.FromResult(value.Dequeue());
		}

		public Task PushAsync(Guid executionId, Command entity)
		{
			if (!_commands.TryGetValue(executionId, out Queue<Command>? value))
			{
				throw new KeyNotFoundException("Commands for this execution ID do not exist.");
			}

			value.Enqueue(entity);
			return Task.CompletedTask;
		}

		public Task UpdateAsync(Guid executionId, Queue<Command> entity)
		{
			if (!_commands.ContainsKey(executionId))
			{
				throw new KeyNotFoundException("Commands for this execution ID do not exist.");
			}

			_commands[executionId] = entity;
			return Task.CompletedTask;
		}

		public Task UpdateFirstAsync(Guid executionId, Command entity)
		{
			if (!_commands.TryGetValue(executionId, out Queue<Command>? value))
			{
				throw new KeyNotFoundException("Commands for this execution ID do not exist.");
			}

			if (value.Count == 0)
			{
				throw new InvalidOperationException("The command queue is empty.");
			}

			var currentElement = value.Peek();
			UpdateFirst(currentElement, entity);

			return Task.CompletedTask;
		}

		private static void UpdateFirst(Command currentState, Command newState)
		{
			currentState.IsValidatedByCommand = newState.IsValidatedByCommand;
			currentState.IsValidatedByMap = newState.IsValidatedByMap;
			currentState.IsValidatedByRobot = newState.IsValidatedByRobot;

			currentState.IsCompletedByCommand = newState.IsCompletedByCommand;
			currentState.IsCompletedByMap = newState.IsCompletedByMap;
			currentState.IsCompletedByRobot = newState.IsCompletedByRobot;
		}
	}
}

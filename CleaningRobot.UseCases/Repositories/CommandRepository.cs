using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;

namespace CleaningRobot.UseCases.Repositories
{
	public class CommandRepository : IQueueRepository<Command>, IRepository<Queue<Command>>
	{
		private readonly Dictionary<Guid, Queue<Command>> _commands = new();
		
		public Task<Queue<Command>> AddAsync(Queue<Command> entity, Guid executionId)
		{
			entity.NotNull();
			_commands.KeyNotExists(executionId);

			_commands[executionId] = entity;
			var result = _commands[executionId];

			return Task.FromResult(result);
		}

		public Task DeleteAsync(Guid executionId)
		{
			_commands.KeyExists(executionId);

			_commands.Remove(executionId);
			return Task.CompletedTask;
		}

		public Task<IEnumerable<Queue<Command>>> GetAllAsync()
		{
			return Task.FromResult<IEnumerable<Queue<Command>>>(_commands.Values);
		}

		public Task<Queue<Command>> GetByIdAsync(Guid executionId)
		{
			_commands.KeyExists(executionId);

			var result = _commands[executionId];
			return Task.FromResult(result);
		}

		public Task<IEnumerable<Command>> ReadAllAsync(Guid executionId)
		{
			_commands.KeyExists(executionId);

			var result = _commands[executionId];
			return Task.FromResult<IEnumerable<Command>>(result);
		}

		public Task<Command?> PeekAsync(Guid executionId)
		{
			_commands.KeyExists(executionId);

			var result = _commands[executionId];

			if (result.Count == 0)
			{
				return Task.FromResult<Command?>(null);
			}
			else 
			{ 
				return Task.FromResult<Command?>(result.Peek());
			}
		}

		public Task<Command?> PullAsync(Guid executionId)
		{
			_commands.KeyExists(executionId);

			var result = _commands[executionId];

			if (result.Count == 0)
			{
				return Task.FromResult<Command?>(null);
			}
			else
			{
				return Task.FromResult<Command?>(result.Dequeue());
			}
		}

		public Task PushAsync(Command entity, Guid executionId)
		{
			_commands.KeyExists(executionId);

			var result = _commands[executionId];
			result.Enqueue(entity);

			return Task.CompletedTask;
		}

		public Task<Queue<Command>> UpdateAsync(Queue<Command> entity, Guid executionId)
		{
			entity.NotNull();
			_commands.KeyExists(executionId);

			_commands[executionId] = entity;
			var result = _commands[executionId];

			return Task.FromResult(result);
		}

		public Task<Command> UpdateFirstAsync(Command entity, Guid executionId)
		{
			entity.NotNull();
			_commands.KeyExists(executionId);

			var currentElement = _commands[executionId].Peek();

			UpdateItem(currentElement, entity);
			
			var result = _commands[executionId].Peek();

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

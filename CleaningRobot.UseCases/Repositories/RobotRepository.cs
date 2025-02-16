using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Interfaces;

namespace CleaningRobot.UseCases.Repositories
{
	public class RobotRepository : IRepository<Robot>
	{
		private readonly Dictionary<Guid, Robot> _robots = [];

		public Task AddAsync(Guid executionId, Robot entity)
		{
			if (_robots.ContainsKey(executionId))
			{
				throw new InvalidOperationException("Robot for this execution ID already exists.");
			}

			_robots[executionId] = entity;
			return Task.CompletedTask;
		}

		public Task DeleteAsync(Guid executionId)
		{
			if (!_robots.ContainsKey(executionId))
			{
				throw new KeyNotFoundException("Robot for this execution ID does not exist.");
			}

			_robots.Remove(executionId);
			return Task.CompletedTask;
		}

		public Task<IEnumerable<Robot>> GetAllAsync()
		{
			return Task.FromResult<IEnumerable<Robot>>(_robots.Values);
		}

		public Task<Robot> GetByIdAsync(Guid executionId)
		{
			if (!_robots.TryGetValue(executionId, out Robot? value))
			{
				throw new KeyNotFoundException("Robot for this execution ID does not exist.");
			}

			return Task.FromResult(value);
		}

		public Task UpdateAsync(Guid executionId, Robot entity)
		{
			if (!_robots.ContainsKey(executionId))
			{
				throw new KeyNotFoundException("Robot for this execution ID does not exist.");
			}

			_robots[executionId] = entity;
			return Task.CompletedTask;
		}
	}
}

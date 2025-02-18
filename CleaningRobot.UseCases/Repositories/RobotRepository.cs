using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;

namespace CleaningRobot.UseCases.Repositories
{
	public class RobotRepository : IRepository<Robot>
	{
		private readonly Dictionary<Guid, Robot> _robots = [];

		public Task<Robot> AddAsync(Robot entity, Guid executionId)
		{
			_robots.KeyNotExists(executionId);

			_robots[executionId] = entity;
			var result = _robots[executionId];

			return Task.FromResult(result);
		}

		public Task DeleteAsync(Guid executionId)
		{
			_robots.KeyExists(executionId);

			_robots.Remove(executionId);
			return Task.CompletedTask;
		}

		public Task<IEnumerable<Robot>> GetAllAsync()
		{
			return Task.FromResult<IEnumerable<Robot>>(_robots.Values);
		}

		public Task<Robot> GetByIdAsync(Guid executionId)
		{
			_robots.KeyExists(executionId);

			var value = _robots[executionId];
			return Task.FromResult(value);
		}

		public Task<Robot> UpdateAsync(Robot entity, Guid executionId)
		{
			entity.NotNull();
			_robots.KeyExists(executionId);

			_robots[executionId] = entity;
			var result = _robots[executionId];

			return Task.FromResult(result);
		}
	}
}

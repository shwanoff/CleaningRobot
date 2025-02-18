using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;

namespace CleaningRobot.UseCases.Repositories
{
	public class MapRepository : IRepository<Map>
	{
		private readonly Dictionary<Guid, Map> _maps = [];
		public Task<Map> AddAsync(Map entity, Guid executionId)
		{
			_maps.KeyNotExists(executionId);

			_maps[executionId] = entity;
			var result = _maps[executionId];


			return Task.FromResult(result);
		}

		public Task DeleteAsync(Guid executionId)
		{
			_maps.KeyExists(executionId);

			_maps.Remove(executionId);
			return Task.CompletedTask;
		}

		public Task<IEnumerable<Map>> GetAllAsync()
		{
			return Task.FromResult<IEnumerable<Map>>(_maps.Values);
		}

		public Task<Map> GetByIdAsync(Guid executionId)
		{
			_maps.KeyExists(executionId);

			var result = _maps[executionId];
			return Task.FromResult(result);
		}

		public Task<Map> UpdateAsync(Map entity, Guid executionId)
		{
			entity.NotNull();
			_maps.KeyExists(executionId);

			_maps[executionId] = entity;
			var result = _maps[executionId];

			return Task.FromResult(result);
		}
	}
}

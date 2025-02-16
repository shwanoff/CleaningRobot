using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Interfaces;

namespace CleaningRobot.UseCases.Repositories
{
	public class MapRepository : IRepository<Map>
	{
		private readonly Dictionary<Guid, Map> _maps = [];
		public Task AddAsync(Guid executionId, Map entity)
		{
			if (_maps.ContainsKey(executionId))
			{
				throw new InvalidOperationException("Map for this execution ID already exists.");
			}

			_maps[executionId] = entity;
			return Task.CompletedTask;
		}

		public Task DeleteAsync(Guid executionId)
		{
			if (!_maps.ContainsKey(executionId))
			{
				throw new KeyNotFoundException("Map for this execution ID does not exist.");
			}

			_maps.Remove(executionId);
			return Task.CompletedTask;
		}

		public Task<IEnumerable<Map>> GetAllAsync()
		{
			return Task.FromResult<IEnumerable<Map>>(_maps.Values);
		}

		public Task<Map> GetByIdAsync(Guid executionId)
		{
			if (!_maps.TryGetValue(executionId, out Map? value))
			{
				throw new KeyNotFoundException("Map for this execution ID does not exist.");
			}

			return Task.FromResult(value);
		}

		public Task UpdateAsync(Guid executionId, Map entity)
		{
			if (!_maps.ContainsKey(executionId))
			{
				throw new KeyNotFoundException("Map for this execution ID does not exist.");
			}

			_maps[executionId] = entity;
			return Task.CompletedTask;
		}
	}
}

using CleaningRobot.Entities.Entities;

namespace CleaningRobot.UseCases.Interfaces
{
	public interface IRepository<T> where T : class
	{
		Task<T> GetByIdAsync(Guid executionId);
		Task<IEnumerable<T>> GetAllAsync();
		Task AddAsync(Guid executionId, T entity);
		Task UpdateAsync(Guid executionId, T entity);
		Task DeleteAsync(Guid executionId);
	}
}

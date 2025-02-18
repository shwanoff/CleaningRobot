using CleaningRobot.Entities.Entities;

namespace CleaningRobot.UseCases.Interfaces.Repositories
{
	public interface IRepository<T> where T : class
	{
		Task<T> GetByIdAsync(Guid executionId);
		Task<IEnumerable<T>> GetAllAsync();
		Task<T> AddAsync(T entity, Guid executionId);
		Task<T> UpdateAsync(T entity, Guid executionId);
		Task DeleteAsync(Guid executionId);
	}
}

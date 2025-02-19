namespace CleaningRobot.UseCases.Interfaces.Repositories
{
	/// <summary>
	/// Provides methods for performing CRUD operations on entities.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	public interface IRepository<T> where T : class
	{
		/// <summary>
		/// Retrieves an entity by its execution identifier asynchronously.
		/// </summary>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the retrieved entity.</returns>
		Task<T> GetByIdAsync(Guid executionId);

		/// <summary>
		/// Retrieves all entities asynchronously.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation. The task result contains a collection of all entities.</returns>
		Task<IEnumerable<T>> GetAllAsync();

		/// <summary>
		/// Adds a new entity asynchronously.
		/// </summary>
		/// <param name="entity">The entity to add.</param>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
		Task<T> AddAsync(T entity, Guid executionId);

		/// <summary>
		/// Updates an existing entity asynchronously.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the updated entity.</returns>
		Task<T> UpdateAsync(T entity, Guid executionId);

		/// <summary>
		/// Deletes an entity by its execution identifier asynchronously.
		/// </summary>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		Task DeleteAsync(Guid executionId);
	}
}
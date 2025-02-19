namespace CleaningRobot.UseCases.Interfaces.Repositories
{
	/// <summary>
	/// Provides methods for performing queue operations on entities.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	public interface IQueueRepository<T> : IRepository<Queue<T>> where T : class
	{
		/// <summary>
		/// Retrieves the first entity in the queue without removing it asynchronously.
		/// </summary>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the first entity in the queue, or null if the queue is empty.</returns>
		Task<T?> PeekAsync(Guid executionId);

		/// <summary>
		/// Retrieves all entities in the queue asynchronously.
		/// </summary>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a collection of all entities in the queue.</returns>
		Task<IEnumerable<T>> ReadAllAsync(Guid executionId);

		/// <summary>
		/// Adds a new entity to the queue asynchronously.
		/// </summary>
		/// <param name="entity">The entity to add.</param>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		Task PushAsync(T entity, Guid executionId);

		/// <summary>
		/// Updates the first entity in the queue asynchronously.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the updated entity.</returns>
		Task<T> UpdateFirstAsync(T entity, Guid executionId);

		/// <summary>
		/// Retrieves and removes the first entity in the queue asynchronously.
		/// </summary>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the first entity in the queue, or null if the queue is empty.</returns>
		Task<T?> PullAsync(Guid executionId);
	}
}
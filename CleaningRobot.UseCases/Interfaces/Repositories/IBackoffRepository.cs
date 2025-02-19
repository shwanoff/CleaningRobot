using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Input;

namespace CleaningRobot.UseCases.Interfaces.Repositories
{
	/// <summary>
	/// Provides methods for performing backoff strategy operations on command queues.
	/// </summary>
	public interface IBackoffRepository : IQueueRepository<Queue<Command>>
	{
		/// <summary>
		/// Initializes the backoff strategy with the given commands asynchronously.
		/// </summary>
		/// <param name="entity">The collection of command collections to initialize the backoff strategy with.</param>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the initialized queue of command queues.</returns>
		Task<Queue<Queue<Command>>> Initialize(IEnumerable<IEnumerable<Command>> entity, Guid executionId);

		/// <summary>
		/// Refreshes the backoff strategy asynchronously.
		/// </summary>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the refreshed queue of command queues.</returns>
		Task<Queue<Queue<Command>>> Refresh(Guid executionId);

		/// <summary>
		/// Updates the first command in the queue asynchronously.
		/// </summary>
		/// <param name="entity">The command to update.</param>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the updated command.</returns>
		Task<Command> UpdateFirstAsync(Command entity, Guid executionId);

		/// <summary>
		/// Gets or sets the settings for the backoff strategy.
		/// </summary>
		CommandSettingsDto Settings { get; set; }
	}
}
using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Interfaces.Controllers
{
	/// <summary>
	/// Provides methods to create and retrieve data transfer objects (DTOs) for robot commands.
	/// </summary>
	/// <typeparam name="TInput">The type of the input data transfer object.</typeparam>
	/// <typeparam name="TOutput">The type of the output data transfer object.</typeparam>
	public interface IController<in TInput, TOutput>
		where TInput : DataDtoBase
		where TOutput : StatusDtoBase
	{
		/// <summary>
		/// Creates a new data transfer object asynchronously.
		/// </summary>
		/// <param name="data">The input data transfer object.</param>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the created output data transfer object.</returns>
		Task<TOutput> CreateAsync(TInput data, Guid executionId);

		/// <summary>
		/// Retrieves a data transfer object asynchronously based on the execution identifier.
		/// </summary>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the retrieved output data transfer object.</returns>
		Task<TOutput> GetAsync(Guid executionId);
	}
}
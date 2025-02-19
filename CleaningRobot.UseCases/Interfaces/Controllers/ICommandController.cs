using CleaningRobot.UseCases.Dto.Input;
using CleaningRobot.UseCases.Dto.Output;

namespace CleaningRobot.UseCases.Interfaces.Controllers
{
	/// <summary>
	/// Provides methods to control the execution of robot commands.
	/// </summary>
	public interface ICommandController : IController<CommandDataDto, CommandCollectionStatusDto>
	{
		/// <summary>
		/// Executes all robot commands asynchronously.
		/// </summary>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the result of the execution as a string.</returns>
		Task<string> ExcecuteAllAsync(Guid executionId);
	}
}
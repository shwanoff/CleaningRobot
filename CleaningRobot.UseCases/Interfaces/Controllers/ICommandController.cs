using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto;

namespace CleaningRobot.UseCases.Interfaces.Controllers
{
	/// <summary>
	/// Command controller interface. Responsible for external command operations
	/// </summary>
	public interface ICommandController
	{
		/// <summary>
		/// Create commands
		/// </summary>
		/// <param name="commands">A collection of command strings to be created</param>
		void Create(IEnumerable<string> commands);

		/// <summary>
		/// Execute all commands
		/// </summary>
		void ExcecuteAll();

		/// <summary>
		/// Execute next command
		/// </summary>
		/// <returns>Returns the status of the executed command</returns>
		CommandStatusDto ExecuteNext();
	}
}

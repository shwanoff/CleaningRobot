using CleaningRobot.Entities.Entities;

namespace CleaningRobot.UseCases.Interfaces.Operations
{
	/// <summary>
	/// Command operation interface. Responsible for internal command operations
	/// </summary>
	internal interface ICommandOperation
    {
		/// <summary>
		/// Validates the given command.
		/// </summary>
		/// <param name="command">The command to validate.</param>
		/// <param name="error">The error message if the command is invalid.</param>
		/// <returns>True if the command is valid, otherwise false.</returns>
		bool ValidateCommand(Command command, out string? error);
	}
}

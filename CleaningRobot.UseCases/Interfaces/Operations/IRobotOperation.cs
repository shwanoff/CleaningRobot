using CleaningRobot.Entities.Entities;

namespace CleaningRobot.UseCases.Interfaces.Operations
{
	/// <summary>
	/// Robot operation interface. Responsible for internal robot operations
	/// </summary>
	internal interface IRobotOperation
	{
		/// <summary>
		/// Validates the given command
		/// </summary>
		/// <param name="command">The command to be validated</param>
		/// <param name="error">The error message if validation fails</param>
		/// <returns>True if the command is valid, otherwise false</returns>
		bool ValidateCommand(Command command, out string error);

		/// <summary>
		/// Execute the given command
		/// </summary>
		/// <param name="command">The command to be executed</param>
		void ExecuteCommand(Command command);
	}
}

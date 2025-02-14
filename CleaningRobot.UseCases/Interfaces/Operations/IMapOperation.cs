﻿using CleaningRobot.Entities.Entities;

namespace CleaningRobot.UseCases.Interfaces.Operations
{
	/// <summary>
	/// Map operation interface. Responsible for internal map operations
	/// </summary>
	internal interface IMapOperation
	{
		/// <summary>
		/// Updates the map based on the given command.
		/// </summary>
		/// <param name="command">The command to update the map with.</param>
		void Update(Command command);

		/// <summary>
		/// Validates the given command.
		/// </summary>
		/// <param name="command">The command to validate.</param>
		/// <param name="error">The error message if the command is invalid.</param>
		/// <returns>True if the command is valid, otherwise false.</returns>
		bool ValidateCommand(Command command, out string? error);
	}
}

using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Extensions
{
	/// <summary>
	/// Extension methods for the Command enum
	/// </summary>
	public static class CommandExtensions
    {
		/// <summary>
		/// Converts a string to a Command enum
		/// </summary>
		/// <param name="command"> The command code as a string (TL, TR, A, B, C) or full name (TurnLeft, Turn Right, Advance, Back, Clean) </param>
		/// <returns> The corresponding Command enum value </returns>
		/// <exception cref="ArgumentException"> Thrown when the command code is invalid </exception>
		public static Command ToCommand(this string command)
		{
			string normalizedCommand = command.Replace(" ", "").ToUpper();

			return normalizedCommand switch
			{
				"TL" => Command.TurnLeft,
				"TR" => Command.TurnRight,
				"A" => Command.Advance,
				"B" => Command.Back,
				"C" => Command.Clean,
				"TURNLEFT" => Command.TurnLeft,
				"TURNRIGHT" => Command.TurnRight,
				"ADVANCE" => Command.Advance,
				"BACK" => Command.Back,
				"CLEAN" => Command.Clean,
				_ => throw new ArgumentException($"Command '{command}' is invalid. Valid values are: TL, TR, A, B, C, TurnLeft, Turn Right, Advance, Back, Clean")
			};
		}
	}
}

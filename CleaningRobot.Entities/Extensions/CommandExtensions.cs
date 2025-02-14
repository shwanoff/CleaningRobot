using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Extensions
{
	/// <summary>
	/// Extension methods for the CommandType enum
	/// </summary>
	public static class CommandExtensions
    {
		/// <summary>
		/// Converts a string to a CommandType enum
		/// </summary>
		/// <param name="command"> The command code as a string (TL, TR, A, B, C) or full name (TurnLeft, Turn Right, Advance, Back, Clean) </param>
		/// <returns> The corresponding CommandType enum value </returns>
		/// <exception cref="ArgumentException"> Thrown when the command code is invalid </exception>
		public static CommandType ToCommand(this string command)
		{
			string normalizedCommand = command.Replace(" ", "").ToUpper();

			return normalizedCommand switch
			{
				"TL" => CommandType.TurnLeft,
				"TR" => CommandType.TurnRight,
				"A" => CommandType.Advance,
				"B" => CommandType.Back,
				"C" => CommandType.Clean,
				"TURNLEFT" => CommandType.TurnLeft,
				"TURNRIGHT" => CommandType.TurnRight,
				"ADVANCE" => CommandType.Advance,
				"BACK" => CommandType.Back,
				"CLEAN" => CommandType.Clean,
				_ => throw new ArgumentException($"CommandType '{command}' is invalid. Valid values are: TL, TR, A, B, C, TurnLeft, Turn Right, Advance, Back, Clean")
			};
		}
	}
}

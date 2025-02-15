using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Extensions
{
	public static class CommandExtensions
    {
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

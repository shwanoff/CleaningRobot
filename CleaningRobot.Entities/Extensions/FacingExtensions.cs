using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Extensions
{

	/// <summary>
	/// Extension methods for the Facing enum
	/// </summary>
	public static class FacingExtensions
	{
		/// <summary>
		/// Converts a string to a Facing enum
		/// </summary>
		/// <param name="direction"> The direction code as a string (N, E, S, W) or full name (North, East, South, West) </param>
		/// <returns> The corresponding Facing enum value </returns>
		/// <exception cref="ArgumentException"> Thrown when the direction code is invalid </exception>
		public static Facing ToFacing(this string direction) => direction.ToUpper() switch
		{
			"N" => Facing.North,
			"E" => Facing.East,
			"S" => Facing.South,
			"W" => Facing.West,
			"NORTH" => Facing.North,
			"EAST" => Facing.East,
			"SOUTH" => Facing.South,
			"WEST" => Facing.West,
			_ => throw new ArgumentException($"Direction '{direction}' is invalid. Valid values are: N, E, S, W, North, East, South, West")
		};

		/// <summary>
		/// Converts a char to a Facing enum
		/// </summary>
		/// <param name="direction"> The direction code as a char (N, E, S, W) </param>
		/// <returns> The corresponding Facing enum value </returns>
		/// <exception cref="ArgumentException"> Thrown when the direction code is invalid </exception>
		public static Facing ToFacing(this char direction)
		{
			return direction.ToString().ToFacing();
		}
	}
}

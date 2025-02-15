using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Extensions
{
	public static class FacingExtensions
	{
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

		public static Facing ToFacing(this char direction)
		{
			return direction.ToString().ToFacing();
		}
	}
}

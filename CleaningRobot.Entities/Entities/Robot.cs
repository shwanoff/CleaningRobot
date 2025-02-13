using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Entities
{
	/// <summary>
	/// Represents the cleaning robot
	/// </summary>
	/// <param name="x"> Column </param>
	/// <param name="y"> Row </param>
	/// <param name="facing"> Direction </param>
	/// <param name="battery"> Battery level </param>
	public class Robot(int x, int y, Facing facing, int battery)
	{
		/// <summary>
		/// Column
		/// </summary>
		public int X { get; set; } = x;

		/// <summary>
		/// Row
		/// </summary>
		public int Y { get; set; } = y;

		/// <summary>
		/// Direction
		/// </summary>
		public Facing Facing { get; set; } = facing;

		/// <summary>
		/// Battery level
		/// </summary>
		public int Battery { get; set; } = battery;

		/// <summary>
		/// Default initialization of the robot. 
		/// Starting position (0, 0), facing North, battery level 100
		/// </summary>
		public Robot() : this(0, 0, Facing.North, 100)
		{
		}

		public override string ToString()
		{
			return $"({X}, {Y}) Facing: {Facing}, Battery: {Battery}";
		}
	}
}

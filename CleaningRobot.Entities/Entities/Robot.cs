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
	public class Robot(uint x, uint y, Facing facing, uint battery)
	{
		/// <summary>
		/// Column
		/// </summary>
		public uint X { get; set; } = x;

		/// <summary>
		/// Row
		/// </summary>
		public uint Y { get; set; } = y;

		/// <summary>
		/// Direction
		/// </summary>
		public Facing Facing { get; set; } = facing;

		/// <summary>
		/// Battery level
		/// </summary>
		public uint Battery { get; set; } = battery;

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

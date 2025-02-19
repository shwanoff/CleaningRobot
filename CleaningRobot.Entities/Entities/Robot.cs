using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Entities
{
	/// <summary>
	/// Represents a robot in the cleaning robot system.
	/// </summary>
	public class Robot : EntityBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Robot"/> class with the specified position and battery level.
		/// </summary>
		/// <param name="x">The x-coordinate of the robot's position.</param>
		/// <param name="y">The y-coordinate of the robot's position.</param>
		/// <param name="facing">The direction the robot is facing.</param>
		/// <param name="battery">The battery level of the robot.</param>
		public Robot(int x, int y, Facing facing, int battery)
		{
			Position = new RobotPosition(x, y, facing);
			Battery = battery;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Robot"/> class with default values.
		/// </summary>
		public Robot() : this(0, 0, Facing.North, 100) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Robot"/> class with the specified position and battery level.
		/// </summary>
		/// <param name="position">The position of the robot.</param>
		/// <param name="battery">The battery level of the robot.</param>
		public Robot(RobotPosition position, int battery) : this(position.X, position.Y, position.Facing, battery) { }

		/// <summary>
		/// Gets or sets the position of the robot.
		/// </summary>
		public RobotPosition Position { get; set; }

		/// <summary>
		/// Gets or sets the battery level of the robot.
		/// </summary>
		public int Battery { get; set; }

		/// <summary>
		/// Returns a string that represents the current robot.
		/// </summary>
		/// <returns>A string that represents the current robot.</returns>
		public override string ToString()
		{
			return $"({Position.X}, {Position.Y}) {Position.Facing} {Battery}";
		}
	}
}
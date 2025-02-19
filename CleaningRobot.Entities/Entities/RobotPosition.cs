using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Entities
{
	/// <summary>
	/// Represents the position of the robot, including its facing direction.
	/// </summary>
	public class RobotPosition : Position
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RobotPosition"/> class with the specified coordinates and facing direction.
		/// </summary>
		/// <param name="x">The x-coordinate of the robot's position.</param>
		/// <param name="y">The y-coordinate of the robot's position.</param>
		/// <param name="facing">The direction the robot is facing.</param>
		public RobotPosition(int x, int y, Facing facing) : base(x, y)
		{
			Facing = facing;
		}

		/// <summary>
		/// Gets or sets the direction the robot is facing.
		/// </summary>
		public Facing Facing { get; set; }

		/// <summary>
		/// Returns a string that represents the current robot position.
		/// </summary>
		/// <returns>A string that represents the current robot position.</returns>
		override public string ToString()
		{
			return $"({X}, {Y}) {Facing}";
		}
	}
}
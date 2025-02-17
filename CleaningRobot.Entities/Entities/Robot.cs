using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Entities
{
	public class Robot(int x, int y, Facing facing, int battery) : EntityBase
	{
		public RobotPosition Position { get; set; } = new RobotPosition(x, y, facing);
		public int Battery { get; set; } = battery;

		public Robot() : this(0, 0, Facing.North, 100) { }

		public Robot(RobotPosition position, int battery) : this(position.X, position.Y, position.Facing, battery) { }

		public override string ToString()
		{
			return $"({Position.X}, {Position.Y}) {Position.Facing} {Battery}";
		}
	}
}

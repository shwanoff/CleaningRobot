using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Entities
{
	public class Robot(int x, int y, Facing facing, int battery)
	{
		public RobotPosition Position { get; set; } = new RobotPosition(x, y, facing);
		public int Battery { get; set; } = battery;

		public Robot() : this(0, 0, Facing.North, 100) { }

		public override string ToString()
		{
			return $"({Position.X}, {Position.Y}) Facing: {Position.Facing}, Battery: {Battery}";
		}
	}
}

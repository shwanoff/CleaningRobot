using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Entities
{
	public class RobotPosition(int x, int y, Facing facing) : Position(x, y)
	{
		public Facing Facing { get; set; } = facing;

		override public string ToString()
		{
			return $"({X}, {Y}) {Facing}";
		}
	}
}

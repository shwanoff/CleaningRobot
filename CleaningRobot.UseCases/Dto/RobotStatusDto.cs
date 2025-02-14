using CleaningRobot.Entities.Enums;

namespace CleaningRobot.UseCases.Dto
{
    public class RobotStatusDto
    {
		public int X { get; set; }
		public int Y { get; set; }
		public Facing Facing { get; set; }
		public int Battery { get; set; }
	}
}

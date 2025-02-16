using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Output
{
	public class RobotStatusDto : StatusDtoBase
	{
		public int X { get; set; }
		public int Y { get; set; }
		public Facing Facing { get; set; }
		public int Battery { get; set; }
	}
}

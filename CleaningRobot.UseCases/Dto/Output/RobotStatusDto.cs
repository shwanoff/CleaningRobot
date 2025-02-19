using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Output
{
	public class RobotStatusDto : StatusDtoBase
	{
		public required int X { get; set; }
		public required int Y { get; set; }
		public required Facing Facing { get; set; }
		public required int Battery { get; set; }
	}
}

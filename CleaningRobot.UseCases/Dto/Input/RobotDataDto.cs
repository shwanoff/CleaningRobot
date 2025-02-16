using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Input
{
    public class RobotDataDto : DataDtoBase
    {
		public required int X { get; set; }
		public required int Y { get; set; }
		public required string Facing { get; set; }
		public required int Battery { get; set; }

	}
}

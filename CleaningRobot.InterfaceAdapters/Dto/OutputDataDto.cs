namespace CleaningRobot.InterfaceAdapters.Dto
{
	public class OutputDataDto
	{
		public required List<PositionDto> Visited { get; set; }
		public required List<PositionDto> Cleaned { get; set; }
		public required RobotPositionDto Final { get; set; }
		public int Battery { get; set; }
	}
}

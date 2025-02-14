namespace CleaningRobot.InterfaceAdapters.Dto
{
    public class OutputDataDto
    {
		public List<PositionDto> Visited { get; set; }
		public List<PositionDto> Cleaned { get; set; }
		public RobotPositionDto Final { get; set; }
		public int Battery { get; set; }
	}
}

namespace CleaningRobot.InterfaceAdapters.Dto
{
	public class InputDataDto
    {
		public required string[][] Map { get; set; }
		public required RobotPositionDto Start { get; set; }
		public required List<string> Commands { get; set; }
		public int Battery { get; set; }
	}
}

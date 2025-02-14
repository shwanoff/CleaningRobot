namespace CleaningRobot.InterfaceAdapters.Dto
{
    public class InputDataDto
    {
		public string[][] Map { get; set; }
		public RobotPositionDto Start { get; set; }
		public List<string> Commands { get; set; }
		public int Battery { get; set; }
	}
}

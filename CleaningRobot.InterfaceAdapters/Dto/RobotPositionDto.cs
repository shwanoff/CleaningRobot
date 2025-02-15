namespace CleaningRobot.InterfaceAdapters.Dto
{
	public class RobotPositionDto : PositionDto
    {
		public required string Facing { get; set; }

		override public string ToString()
		{
			return $"({X}, {Y}) Facing: {Facing}";
		}
	}
}

namespace CleaningRobot.InterfaceAdapters.Dto
{
	/// <summary>
	/// Data transfer object for robot position
	/// </summary>
	public class RobotPositionDto : PositionDto
    {
		/// <summary>
		/// Direction
		/// </summary>
		public required string Facing { get; set; }

		override public string ToString()
		{
			return $"({X}, {Y}) Facing: {Facing}";
		}
	}
}

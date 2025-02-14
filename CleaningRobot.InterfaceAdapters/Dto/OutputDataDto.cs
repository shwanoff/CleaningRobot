namespace CleaningRobot.InterfaceAdapters.Dto
{
	/// <summary>
	/// Data transfer object for output data
	/// </summary>
	public class OutputDataDto
    {
		/// <summary>
		/// List of visited positions
		/// </summary>
		public required List<PositionDto> Visited { get; set; }

		/// <summary>
		/// List of cleaned positions
		/// </summary>
		public required List<PositionDto> Cleaned { get; set; }

		/// <summary>
		/// Final position of the robot
		/// </summary>
		public required RobotPositionDto Final { get; set; }

		/// <summary>
		/// Battery level of the robot at the end
		/// </summary>
		public int Battery { get; set; }

		override public string ToString()
		{
			return $"Visited: {Visited}, Cleaned: {Cleaned}, Final: {Final}, Battery: {Battery}";
		}
	}
}

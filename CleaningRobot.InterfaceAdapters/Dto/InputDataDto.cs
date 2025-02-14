namespace CleaningRobot.InterfaceAdapters.Dto
{
	/// <summary>
	/// Data transfer object for input data
	/// </summary>
	public class InputDataDto
    {
		/// <summary>
		/// Room map
		/// </summary>
		public required string[][] Map { get; set; }

		/// <summary>
		/// Starting position of the robot
		/// </summary>
		public required RobotPositionDto Start { get; set; }

		/// <summary>
		/// List of commands
		/// </summary>
		public required List<string> Commands { get; set; }

		/// <summary>
		/// Battery level
		/// </summary>
		public int Battery { get; set; }

		override public string ToString()
		{
			return $"Map: {Map}, Start: {Start}, Commands: {Commands}, Battery: {Battery}";
		}
	}
}

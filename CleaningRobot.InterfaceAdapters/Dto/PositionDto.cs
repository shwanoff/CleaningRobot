namespace CleaningRobot.InterfaceAdapters.Dto
{
	/// <summary>
	/// Data transfer object for position
	/// </summary>
	public class PositionDto
    {
		/// <summary>
		/// Column
		/// </summary>
		public int X { get; set; }

		/// <summary>
		/// Row
		/// </summary>
		public int Y { get; set; }

		override public string ToString()
		{
			return $"({X}, {Y})";
		}
	}
}

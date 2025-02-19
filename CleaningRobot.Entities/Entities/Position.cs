namespace CleaningRobot.Entities.Entities
{
	/// <summary>
	/// Represents a position with x and y coordinates.
	/// </summary>
	public class Position
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Position"/> class with the specified coordinates.
		/// </summary>
		/// <param name="x">The x-coordinate of the position.</param>
		/// <param name="y">The y-coordinate of the position.</param>
		public Position(int x, int y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// Gets or sets the x-coordinate of the position.
		/// </summary>
		public int X { get; set; }

		/// <summary>
		/// Gets or sets the y-coordinate of the position.
		/// </summary>
		public int Y { get; set; }

		/// <summary>
		/// Returns a string that represents the current position.
		/// </summary>
		/// <returns>A string that represents the current position.</returns>
		override public string ToString()
		{
			return $"({X}, {Y})";
		}
	}
}
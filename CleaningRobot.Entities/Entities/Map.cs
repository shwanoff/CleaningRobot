namespace CleaningRobot.Entities.Entities
{
	/// <summary>
	/// Represents a map consisting of cells for the cleaning robot.
	/// </summary>
	public class Map : EntityBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Map"/> class with the specified cells.
		/// </summary>
		/// <param name="cells">A 2D array of cells that make up the map.</param>
		public Map(Cell[,] cells)
		{
			Cells = cells;
		}

		/// <summary>
		/// Gets the width of the map.
		/// </summary>
		public int Width { get { return Cells.GetLength(0); } }

		/// <summary>
		/// Gets the height of the map.
		/// </summary>
		public int Height { get { return Cells.GetLength(1); } }

		/// <summary>
		/// Gets the cells that make up the map.
		/// </summary>
		public Cell[,] Cells { get; private set; }

		/// <summary>
		/// Returns a string that represents the current map.
		/// </summary>
		/// <returns>A string that represents the current map.</returns>
		public override string ToString()
		{
			return $"{Width}x{Height}";
		}
	}
}
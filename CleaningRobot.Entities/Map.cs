using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities
{
	/// <summary>
	/// Represents the map of the environment
	/// </summary>
	/// <param name="cells"> Matrix of the map </param>
	public class Map(Cell[,] cells)
	{
		/// <summary>
		/// Width of the map
		/// </summary>
		public int Width
		{
			get
			{
				return Cells.GetLength(0);
			}
		}

		/// <summary>
		/// Height of the map
		/// </summary>
		public int Height
		{
			get
			{
				return Cells.GetLength(1);
			}
		}

		/// <summary>
		/// Matrix of the map
		/// </summary>
		public Cell[,] Cells { get; private set; } = cells;

		public override string ToString()
		{
			return $"Map: {Width}x{Height}";
		}
	}
}

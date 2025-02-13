using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Extensions
{
	/// <summary>
	/// Extension methods for the Cell enums
	/// </summary>
	public static class CellExtensions
	{
		/// <summary>
		/// Converts a string to a CellType enum
		/// </summary>
		/// <param name="cellType"> The cell type code as a string (S, C, W, null) or full name (CleanableSpace, Column, Wall) </param>
		/// <returns> The corresponding CellType enum value </returns>
		/// <exception cref="ArgumentException"> Thrown when the cellType type code is invalid </exception>
		public static CellType ToCellType(this string cellType)
		{
			if (string.IsNullOrWhiteSpace(cellType))
			{
				return CellType.Wall;
			}

			string normalizedCell = cellType.Replace(" ", "").ToUpper();

			return normalizedCell switch
			{
				"S" => CellType.CleanableSpace,
				"C" => CellType.Column,
				"W" => CellType.Wall,
				"CLEANABLESPACE" => CellType.CleanableSpace,
				"COLUMN" => CellType.Column,
				"WALL" => CellType.Wall,
				"NULL" => CellType.Wall,
				_ => throw new ArgumentException($"CellType '{cellType}' is invalid. Valid values are: S, C, W, CleanableSpace, Column, Wall, null")
			};
		}

		/// <summary>
		/// Converts a string to a CellState enum
		/// </summary>
		/// <param name="cellState"> The cell state code as a string (N, V, C) or full name (NotVisited, Visited, Cleaned) </param>
		/// <returns> The corresponding CellState enum value </returns>
		/// <exception cref="ArgumentException"> Thrown when the cell state code is invalid </exception>

		public static CellState ToCellState(this string cellState)
		{
			string normalizedCell = cellState.Replace(" ", "").ToUpper();

			return normalizedCell switch
			{
				"N" => CellState.NotVisited,
				"V" => CellState.Visited,
				"C" => CellState.Cleaned,
				"NOTVISITED" => CellState.NotVisited,
				"VISITED" => CellState.Visited,
				"CLEANED" => CellState.Cleaned,
				_ => throw new ArgumentException($"CellState '{cellState}' is invalid. Valid values are: N, V, C, NotVisited, Visited, Cleaned")
			};
		}
	}
}

using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Extensions
{
	/// <summary>
	/// Extension methods for the Cell enum
	/// </summary>
	public static class CellExtensions
	{
		/// <summary>
		/// Converts a string to a Cell enum
		/// </summary>
		/// <param name="cell"> The cell code as a string (S, C, W, null) or full name (CleanableSpace, Column, Wall) </param>
		/// <returns> The corresponding Cell enum value </returns>
		/// <exception cref="ArgumentException"> Thrown when the cell code is invalid </exception>
		public static Cell ToCell(this string cell)
		{
			if (string.IsNullOrWhiteSpace(cell))
			{
				return Cell.Wall;
			}

			string normalizedCell = cell.Replace(" ", "").ToUpper();

			return normalizedCell switch
			{
				"S" => Cell.CleanableSpace,
				"C" => Cell.Column,
				"W" => Cell.Wall,
				"CLEANABLESPACE" => Cell.CleanableSpace,
				"COLUMN" => Cell.Column,
				"WALL" => Cell.Wall,
				"NULL" => Cell.Wall,
				_ => throw new ArgumentException($"Cell '{cell}' is invalid. Valid values are: S, C, W, CleanableSpace, Column, Wall, null")
			};
		}
	}
}

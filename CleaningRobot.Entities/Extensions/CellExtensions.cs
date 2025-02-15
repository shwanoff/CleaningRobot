using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Extensions
{
	public static class CellExtensions
	{
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

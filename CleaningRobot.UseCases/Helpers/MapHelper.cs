using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Extensions;

namespace CleaningRobot.UseCases.Helpers
{
    public static class MapHelper
    {
		public static Cell[,] ConvertToRectangularArray(string[][] jaggedArray)
		{
			int rows = jaggedArray.Length;
			int cols = jaggedArray.Max(row => row.Length);
			Cell[,] rectangularArray = new Cell[rows, cols];

			for (int y = 0; y < rows; y++)
			{
				for (int x = 0; x < cols; x++)
				{
					Cell cell;
					if (jaggedArray[y].Length <= x || jaggedArray[y][x] == null)
					{
						cell = new Cell(x, y, CellType.Wall);
					}
					else
					{
						var cellType = jaggedArray[y][x].ToCellType();
						cell = new Cell(x, y, cellType);
					}

					rectangularArray[y, x] = cell;
				}
			}

			return rectangularArray;
		}
	}
}

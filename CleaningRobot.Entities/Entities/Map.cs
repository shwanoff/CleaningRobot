using System.ComponentModel;
using System.Text;

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

		/// <summary>
		/// Generates a string representation of the map, where each cell is represented by its description attribute or type name.
		/// </summary>
		/// <returns>A string that represents the map, with each cell's description or type name, separated by '|' for each row.</returns>
		public string Draw()
		{
			var map = new StringBuilder();
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					var cellType = Cells[x, y].Type;
					var fieldInfo = cellType.GetType().GetField(cellType.ToString());
					var descriptionAttribute = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
					_ = map.Append(descriptionAttribute != null ? descriptionAttribute.Description : cellType.ToString());
				}
				map.Append('|');
			}
			return map.ToString();
		}
	}
}
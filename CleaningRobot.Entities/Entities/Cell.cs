using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Entities
{
	/// <summary>
	/// Represents a cell in the cleaning robot's map.
	/// </summary>
	public class Cell
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Cell"/> class with the specified position and type.
		/// </summary>
		/// <param name="x">The x-coordinate of the cell.</param>
		/// <param name="y">The y-coordinate of the cell.</param>
		/// <param name="type">The type of the cell.</param>
		public Cell(int x, int y, CellType type)
		{
			Position = new Position(x, y);
			Type = type;
			State = CellState.NotVisited;
		}

		/// <summary>
		/// Gets or sets the position of the cell.
		/// </summary>
		public Position Position { get; set; }

		/// <summary>
		/// Gets the type of the cell.
		/// </summary>
		public CellType Type { get; private set; }

		/// <summary>
		/// Gets or sets the state of the cell.
		/// </summary>
		public CellState State { get; set; }

		/// <summary>
		/// Returns a string that represents the current cell.
		/// </summary>
		/// <returns>A string that represents the current cell.</returns>
		public override string ToString()
		{
			return $"({Position.X}, {Position.Y}) {Type} {State}";
		}
	}
}
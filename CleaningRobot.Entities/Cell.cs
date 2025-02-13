using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities
{
	/// <summary>
	/// Represents a cell in the room
	/// </summary>
	/// <param name="x"> Position on X </param>
	/// <param name="y"> Position on Y</param>
	/// <param name="type"> Cell type </param>
	public class Cell(int x, int y, CellType type)
	{
		/// <summary>
		/// Position on X
		/// </summary>
		public int X { get; private set; } = x;

		/// <summary>
		/// Position on Y
		/// </summary>
		public int Y { get; private set; } = y;

		/// <summary>
		/// Cell type
		/// </summary>
		public CellType Type { get; private set; } = type;

		/// <summary>
		/// Cell state
		/// </summary>
		public CellState State { get; set; } = CellState.NotVisited;

		public override string ToString()
		{
			return $"({X}, {Y}) Type:{Type}, State: {State}";
		}
	}
}

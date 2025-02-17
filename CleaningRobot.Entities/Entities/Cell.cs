using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Entities
{
	public class Cell(int x, int y, CellType type)
	{
		public Position Position { get; set; } = new Position(x, y);
		public CellType Type { get; private set; } = type;
		public CellState State { get; set; } = CellState.NotVisited;

		public override string ToString()
		{
			return $"({Position.X}, {Position.Y}) {Type} {State}";
		}
	}
}

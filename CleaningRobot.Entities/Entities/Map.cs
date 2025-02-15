namespace CleaningRobot.Entities.Entities
{
	public class Map(Cell[,] cells)
	{
		public int Width { get { return Cells.GetLength(0); } }
		public int Height { get	{ return Cells.GetLength(1); } }
		public Cell[,] Cells { get; private set; } = cells;

		public override string ToString()
		{
			return $"Map: {Width}x{Height}";
		}
	}
}

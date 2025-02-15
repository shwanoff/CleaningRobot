namespace CleaningRobot.Entities.Entities
{
    public class Position(int x, int y)
	{
		public int X { get; set; } = x;
		public int Y { get; set; } = y;

		override public string ToString()
		{
			return $"({X}, {Y})";
		}
	}
}

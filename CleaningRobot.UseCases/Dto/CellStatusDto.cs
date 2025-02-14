using CleaningRobot.Entities.Enums;

namespace CleaningRobot.UseCases.Dto
{
    public class CellStatusDto
    {
		public int X { get; set; }
		public int Y { get; set; }
		public CellType Type { get; set; }
		public CellState State { get; set; }
	}
}

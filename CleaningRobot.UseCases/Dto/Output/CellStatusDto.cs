using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Output
{
	public class CellStatusDto : StatusDtoBase
	{
		public int X { get; set; }
		public int Y { get; set; }
		public CellType Type { get; set; }
		public CellState State { get; set; }
	}
}

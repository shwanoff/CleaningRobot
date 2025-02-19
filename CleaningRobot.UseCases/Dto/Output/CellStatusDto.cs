using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Output
{
	public class CellStatusDto : StatusDtoBase
	{
		public required int X { get; set; }
		public required int Y { get; set; }
		public required CellType Type { get; set; }
		public required CellState State { get; set; }
	}
}

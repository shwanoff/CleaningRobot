using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Output
{
    public class MapStatusDto : StatusDtoBase
    {
		public required IEnumerable<CellStatusDto> Cells { get; set; }
		public required int Width { get; set; }
		public required int Height { get; set; }

	}
}

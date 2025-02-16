using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Input
{
    public class MapDataDto : DataDtoBase
    {
        public required string[][] Map { get; set; }

	}
}

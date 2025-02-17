using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Input
{
	public class MapDataDto : DataDtoBase
	{
		public required string[][] Map { get; set; }

		#if DEBUG
		public override string ToString()
		{
			return $"{string.Join(", ", Map.Select(row => string.Join(" ", row)))}";
		}
		#endif
	}
}

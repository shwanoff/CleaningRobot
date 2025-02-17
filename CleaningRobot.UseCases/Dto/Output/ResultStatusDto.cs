using CleaningRobot.UseCases.Dto.Base;
using CleaningRobot.UseCases.Enums;

namespace CleaningRobot.UseCases.Dto.Output
{
    public class ResultStatusDto : StatusDtoBase
	{
		public required ResultState State { get; set; }
	}
}

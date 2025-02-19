using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Output
{
    public class CommandCollectionStatusDto : StatusDtoBase
	{
        public required IReadOnlyList<CommandStatusDto> Commands { get; set; }
	}
}

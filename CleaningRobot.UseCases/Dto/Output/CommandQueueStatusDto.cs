using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Output
{
    public class CommandQueueStatusDto : StatusDtoBase
	{
        public required Queue<CommandStatusDto> Commands { get; set; }
	}
}

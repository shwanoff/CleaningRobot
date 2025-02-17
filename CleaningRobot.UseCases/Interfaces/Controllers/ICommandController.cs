using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Input;
using CleaningRobot.UseCases.Dto.Output;

namespace CleaningRobot.UseCases.Interfaces.Controllers
{
	public interface ICommandController : IController<CommandDataDto, CommandQueueStatusDto>
	{
		Task<string> ExcecuteAllAsync(Guid executionId);
	}
}

using CleaningRobot.UseCases.Dto.Input;
using CleaningRobot.UseCases.Dto.Output;

namespace CleaningRobot.UseCases.Interfaces.Controllers
{
	public interface IRobotController : IController<RobotDataDto, RobotStatusDto>
	{
		
	}
}

using CleaningRobot.UseCases.Dto;

namespace CleaningRobot.UseCases.Interfaces.Controllers
{
    public interface IRobotController
    {
        void Create(int x, int y, string facing, int battery);
        RobotStatusDto GetCurrentStatus();
	}
}

using CleaningRobot.Entities.Enums;

namespace CleaningRobot.UseCases.Interfaces
{
    public interface IRobotController
    {
        void ValidateCommand(Command command);
		void ExecuteCommand(Command command);
	}
}

using CleaningRobot.Entities.Entities;

namespace CleaningRobot.UseCases.Interfaces.Operations
{
    public interface IRobotOperation
    {
		bool ValidateCommand(Command command, out string error);
		void ExecuteCommand(Command command);
	}
}

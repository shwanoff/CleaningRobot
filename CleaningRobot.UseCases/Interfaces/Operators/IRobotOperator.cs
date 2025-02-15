using CleaningRobot.Entities.Entities;

namespace CleaningRobot.UseCases.Interfaces.Operators
{
	internal interface IRobotOperator
	{
		bool ValidateCommand(Command command, out string? error);
		void ExecuteCommand(Command command);
		RobotPosition GetRobotPosition();
	}
}

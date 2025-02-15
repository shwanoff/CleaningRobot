using CleaningRobot.Entities.Entities;

namespace CleaningRobot.UseCases.Interfaces.Operators
{
	internal interface ICommandOperator
	{
		bool ValidateCommand(Command command, out string? error);
	}
}

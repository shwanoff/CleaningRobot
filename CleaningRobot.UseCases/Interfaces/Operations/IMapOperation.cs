using CleaningRobot.Entities.Entities;

namespace CleaningRobot.UseCases.Interfaces.Operations
{
    public interface IMapOperation
    {
		void Update(Command command);
		bool ValidateCommand(Command command, out string error);
	}
}

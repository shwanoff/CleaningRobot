using CleaningRobot.Entities.Entities;

namespace CleaningRobot.UseCases.Interfaces.Operators
{
	internal interface IMapOperator
	{
		void Update(Command command);
		bool ValidateCommand(Command command, out string? error);
		Cell GetCellStatus(int x, int y);

		int GetMapWidth();
		int GetMapHeight();
	}
}

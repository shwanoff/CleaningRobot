using CleaningRobot.UseCases.Dto;

namespace CleaningRobot.UseCases.Interfaces.Controllers
{
	public interface ICommandController
	{
		void Create(IEnumerable<string> commands);
		void ExcecuteAll();
		CommandStatusDto ExecuteNext();
	}
}

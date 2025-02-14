using CleaningRobot.Entities.Entities;

namespace CleaningRobot.UseCases.Interfaces.Controllers
{
    public interface ICommandController
    {
		void Create(IEnumerable<string> commands);
		void ExcecuteAll();
		void ExecuteNext();
	}
}

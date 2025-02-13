using CleaningRobot.Entities.Enums;

namespace CleaningRobot.UseCases.Interfaces
{
    public interface IMapController
    {
        void Create();
		void Update();
		void ValidateCommand(Command command);
        
	}
}

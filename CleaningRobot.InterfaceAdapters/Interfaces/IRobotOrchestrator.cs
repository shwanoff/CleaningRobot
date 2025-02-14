using CleaningRobot.Entities.Entities;

namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	public interface IRobotOrchestrator
	{
		ExecutionResult Execute(string inputFilePath, string outputFilePath);
	}
}

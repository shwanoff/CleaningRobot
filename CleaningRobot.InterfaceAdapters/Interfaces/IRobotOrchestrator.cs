using CleaningRobot.InterfaceAdapters.Dto;

namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	public interface IRobotOrchestrator
	{
		ExecutionResultDto Execute(string inputFilePath, string outputFilePath);
	}
}

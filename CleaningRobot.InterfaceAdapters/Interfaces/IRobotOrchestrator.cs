using CleaningRobot.InterfaceAdapters.Dto;

namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	public interface IRobotOrchestrator
	{
		Task<ExecutionResultDto> ExecuteAsync(string inputFilePath, string outputFilePath);
	}
}

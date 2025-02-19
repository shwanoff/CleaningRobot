using CleaningRobot.InterfaceAdapters.Dto;

namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	/// <summary>
	/// Provides methods to orchestrate the execution of robot commands.
	/// </summary>
	public interface IRobotOrchestrator
	{
		/// <summary>
		/// Executes the robot commands based on the input file and writes the results to the output file asynchronously.
		/// </summary>
		/// <param name="inputFilePath">The path to the input file containing the robot commands.</param>
		/// <param name="outputFilePath">The path to the output file where the results will be written.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the execution result.</returns>
		Task<ExecutionResultDto> ExecuteAsync(string inputFilePath, string outputFilePath);
	}
}
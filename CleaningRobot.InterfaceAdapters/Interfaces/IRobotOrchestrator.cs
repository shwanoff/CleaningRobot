using CleaningRobot.Entities.Entities;

namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	/// <summary>
	/// Main entry point for the robot control
	/// </summary>
	public interface IRobotOrchestrator
	{
		/// <summary>
		/// Execute robot commands
		/// </summary>
		/// <param name="inputFilePath"> Path to input JSON file with map, commands and robot parameters </param>
		/// <param name="outputFilePath"> Path to output JSON file with list of cleaned/visited points and robot status at the end </param>
		/// <returns> Results of execution list of cleaned/visited points and robot status, same as in output file </returns>
		ExecutionResultDto Execute(string inputFilePath, string outputFilePath);
	}
}

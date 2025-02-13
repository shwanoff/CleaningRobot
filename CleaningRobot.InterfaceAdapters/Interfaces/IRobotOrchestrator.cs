namespace CleaningRobot.InterfaceAdapters.Interfaces
{
    public interface IRobotOrchestrator
    {
		bool Execute(string inputFilePath, string outputFilePath);
	}
}

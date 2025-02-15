namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	public interface ILogAdapter
    {
		void Info(string message);
		void Warning(string message);
		void Error(string message, Exception? exception = null);
	}
}

namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	public interface ILogAdapter
	{
		Task InfoAsync(string message, Guid executionId);
		Task WarningAsync(string message, Guid executionId);
		Task ErrorAsync(string message, Guid executionId, Exception? exception = null);
	}
}

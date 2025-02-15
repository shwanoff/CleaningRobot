namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	public interface IJsonAdapter
    {
		bool TrySerialize<T>(T item, out string result);
		bool TryDeserialize<T>(string json, out T result);
	}
}

namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	public interface IJsonAdapter
	{
		Task<string> SerializeAsync<T>(T item);
		Task<T> DeserializeAsync<T>(string json);
	}
}

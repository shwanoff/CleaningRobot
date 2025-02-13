namespace CleaningRobot.InterfaceAdapters.Interfaces
{
    public interface IJsonAdapter
    {
		string Serialize<T>(T item);
		T Deserialize<T>(string json);
	}
}

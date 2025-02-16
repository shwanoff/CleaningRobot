using CleaningRobot.InterfaceAdapters.Interfaces;
using System.Text.Json;

namespace CleaningRobot.InterfaceAdapters.Adapters
{
	public class JsonAdapter : IJsonAdapter
	{
		public async Task<T> DeserializeAsync<T>(string json)
		{
			if (string.IsNullOrWhiteSpace(json))
			{
				throw new ArgumentNullException(nameof(json), "Json to deserialize cannot be null or empty");
			}

			var result = await Task.Run(() => JsonSerializer.Deserialize<T>(json));

			if (result == null)
			{
				throw new ArgumentException("Error deserializing json");
			}

			return result;
		}

		public async Task<string> SerializeAsync<T>(T? item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item), "Item to serialize cannot be null");
			}

			var result = await Task.Run(() => JsonSerializer.Serialize(item));

			if (string.IsNullOrWhiteSpace(result))
			{
				throw new ArgumentException("Error serializing item");
			}

			return result;
		}
	}
}

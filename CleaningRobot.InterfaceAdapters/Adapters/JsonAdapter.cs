using CleaningRobot.InterfaceAdapters.Interfaces;
using System.Text.Json;
using System.Text.RegularExpressions;

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

			var normalizedJson = Normalize(json);

			var result = await Task.Run(() => JsonSerializer.Deserialize<T>(normalizedJson, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			}));

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

		public static string Normalize(string json)
		{
			json = Regex.Replace(json, "\"null\"", "null", RegexOptions.IgnoreCase);

			return json;
		}
	}
}

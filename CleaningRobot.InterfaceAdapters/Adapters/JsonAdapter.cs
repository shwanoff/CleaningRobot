using CleaningRobot.InterfaceAdapters.Interfaces;
using System.Text.Json;

namespace CleaningRobot.InterfaceAdapters.Adapters
{
	public class JsonAdapter : IJsonAdapter
	{
		public bool TryDeserialize<T>(string json, out T? result)
		{
			result = default;

			if (string.IsNullOrWhiteSpace(json))
			{
				return false;
			}
			try
			{
				var deserializedResult = JsonSerializer.Deserialize<T>(json);

				if (deserializedResult == null)
				{
					return false;
				}

				result = deserializedResult;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool TrySerialize<T>(T? item, out string result)
		{
			result = string.Empty;

			if (item == null)
			{
				return false;
			}

			try
			{
				result = JsonSerializer.Serialize(item);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}

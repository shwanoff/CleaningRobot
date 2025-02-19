using CleaningRobot.Entities.Enums;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CleaningRobot.InterfaceAdapters.JsonConverters
{
	public class FacingJsonConverter : JsonConverter<Facing>
	{
		public override Facing Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var description = reader.GetString();
			if (description == null)
			{
				throw new JsonException("Facing value is null");
			}

			foreach (var field in typeof(Facing).GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				var attribute = field.GetCustomAttribute<DescriptionAttribute>();
				if (attribute != null && attribute.Description == description)
				{
					return (Facing)field.GetValue(null);
				}
			}

			throw new JsonException($"Unknown Facing value: {description}");
		}

		public override void Write(Utf8JsonWriter writer, Facing value, JsonSerializerOptions options)
		{
			var field = typeof(Facing).GetField(value.ToString());
			var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
			var description = attribute?.Description ?? value.ToString();
			writer.WriteStringValue(description);
		}
	}
}

namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	/// <summary>
	/// JSON adapter interface
	/// </summary>
	public interface IJsonAdapter
    {
		/// <summary>
		/// Try to serialize object to JSON
		/// </summary>
		/// <typeparam name="T"> Data type of the item </typeparam>
		/// <param name="item"> Item that must be serialized </param>
		/// <param name="result"> Result JSON string with the serialized item </param>
		/// <returns> true - successful serialization, false - cannot serialize the item </returns>
		bool TrySerialize<T>(T item, out string result);

		/// <summary>
		/// Try to deserialize JSON to object
		/// </summary>
		/// <typeparam name="T"> Data type of the item </typeparam>
		/// <param name="json"> JSON string with serialized item </param>
		/// <param name="result"> Deserialized item </param>
		/// <returns> true - successful deserialization, false - cannot deserialize the item </returns>
		bool TryDeserialize<T>(string json, out T result);
	}
}

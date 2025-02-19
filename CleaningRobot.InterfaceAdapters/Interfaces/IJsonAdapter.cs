namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	/// <summary>
	/// Provides methods for serializing and deserializing objects to and from JSON.
	/// </summary>
	public interface IJsonAdapter
	{
		/// <summary>
		/// Serializes the specified object to a JSON string asynchronously.
		/// </summary>
		/// <typeparam name="T">The type of the object to serialize.</typeparam>
		/// <param name="item">The object to serialize.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the JSON string representation of the object.</returns>
		Task<string> SerializeAsync<T>(T item);

		/// <summary>
		/// Deserializes the specified JSON string to an object of type <typeparamref name="T"/> asynchronously.
		/// </summary>
		/// <typeparam name="T">The type of the object to deserialize to.</typeparam>
		/// <param name="json">The JSON string to deserialize.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object.</returns>
		Task<T> DeserializeAsync<T>(string json);
	}
}
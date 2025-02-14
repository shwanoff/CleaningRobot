namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	/// <summary>
	/// File adapter interface
	/// </summary>
	public interface IFileAdapter
    {
		/// <summary>
		/// Check if file exists
		/// </summary>
		/// <param name="path"> Full path to the file </param>
		/// <returns> true - file exists, false - file do not exists </returns>
		bool Exists(string path);

		/// <summary>
		/// Check if path is valid
		/// </summary>
		/// <param name="path"> Full path to the file </param>
		/// <returns> true - file path is valid, false - file path is invalid </returns>
		bool IsPath(string path);

		/// <summary>
		/// Read file content
		/// </summary>
		/// <param name="path"> Full path to the file </param>
		/// <param name="content"> All data from the file </param>
		/// <returns> true - successful reading, false - cannot read the file </returns>
		bool TryRead(string path, out string content);

		/// <summary>
		/// Write content to the file
		/// </summary>
		/// <param name="path"> Full path to the file </param>
		/// <param name="content"> Data that must be written </param>
		/// <param name="replase"> Override existing file </param>
		/// <returns> true - successful writting, false cannot write the file </returns>
		bool TryWrite(string path, string content, bool replase = true);
	}
}

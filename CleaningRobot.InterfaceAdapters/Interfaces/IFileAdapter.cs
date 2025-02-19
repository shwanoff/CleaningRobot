namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	/// <summary>
	/// Defines methods for file operations such as validation, reading, and writing.
	/// </summary>
	public interface IFileAdapter
	{
		/// <summary>
		/// Validates the input file path.
		/// </summary>
		/// <param name="path">The file path to validate.</param>
		/// <param name="error">The error message if validation fails.</param>
		/// <param name="mustExist">Indicates whether the file must exist.</param>
		/// <returns>True if the file path is valid; otherwise, false.</returns>
		bool ValidateInput(string path, out string? error, bool mustExist = false);

		/// <summary>
		/// Reads the content of the file asynchronously.
		/// </summary>
		/// <param name="path">The file path to read.</param>
		/// <returns>A task that represents the asynchronous read operation. The task result contains the file content.</returns>
		Task<string> ReadAsync(string path);

		/// <summary>
		/// Writes content to the file asynchronously.
		/// </summary>
		/// <param name="path">The file path to write to.</param>
		/// <param name="content">The content to write to the file.</param>
		/// <param name="replace">Indicates whether to replace the file if it already exists.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		Task WriteAsync(string path, string content, bool replace = true);
	}
}
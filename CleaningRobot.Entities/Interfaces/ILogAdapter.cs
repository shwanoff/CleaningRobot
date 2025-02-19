namespace CleaningRobot.Entities.Interfaces
{
	/// <summary>
	/// Provides methods for logging messages with different severity levels.
	/// </summary>
	public interface ILogAdapter
	{
		/// <summary>
		/// Logs a trace message asynchronously.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		Task TraceAsync(string message, Guid executionId);

		/// <summary>
		/// Logs a debug message asynchronously.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		Task DebugAsync(string message, Guid executionId);

		/// <summary>
		/// Logs an informational message asynchronously.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		Task InfoAsync(string message, Guid executionId);

		/// <summary>
		/// Logs a warning message asynchronously.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		Task WarningAsync(string message, Guid executionId);

		/// <summary>
		/// Logs an error message asynchronously.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="executionId">The unique identifier for the execution context.</param>
		/// <param name="exception">The exception to log (optional).</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		Task ErrorAsync(string message, string location, Guid executionId, Exception? exception = null);
	}
}
namespace CleaningRobot.InterfaceAdapters.Interfaces
{
	/// <summary>
	/// Log adapter interface
	/// </summary>
	public interface ILogAdapter
    {
		/// <summary>
		/// Log information message
		/// </summary>
		/// <param name="message"> Trace message </param>
		void Info(string message);

		/// <summary>
		/// Log warning message
		/// </summary>
		/// <param name="message"> Important, but not critical message </param>
		void Warning(string message);

		/// <summary>
		/// Log error message
		/// </summary>
		/// <param name="message"> Error message </param>
		/// <param name="exception"> Exception if exists </param>
		void Error(string message, Exception? exception = null);
	}
}

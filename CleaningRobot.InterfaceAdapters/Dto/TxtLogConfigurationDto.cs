namespace CleaningRobot.InterfaceAdapters.Dto
{
    public class TxtLogConfigurationDto
    {
		public required LogLevelConfiguration LogLevel { get; set; }
		public required TextLogConfiguration TextLog { get; set; }
	}

	public class LogLevelConfiguration
	{
		public required bool Info { get; set; }
		public required bool Warning { get; set; }
		public required bool Error { get; set; }
	}

	public class TextLogConfiguration
	{
		public required string Path { get; set; }
		public required string FileNameFormat { get; set; }
	}
}

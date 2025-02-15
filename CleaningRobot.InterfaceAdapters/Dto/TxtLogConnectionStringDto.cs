namespace CleaningRobot.InterfaceAdapters.Dto
{
    public class TxtLogConnectionStringDto
    {
		public required string FolderPath { get; set; }
		public bool WriteTrace { get; set; }
	}
}

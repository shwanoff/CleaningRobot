using CleaningRobot.Entities.Enums;

namespace CleaningRobot.Entities.Entities
{
    public class Command
    {
        public CommandType CommandType { get; set; }
        public int ConsumedEnergy { get; set; }
	}
}

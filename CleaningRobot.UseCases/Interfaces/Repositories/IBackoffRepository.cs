using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Input;

namespace CleaningRobot.UseCases.Interfaces.Repositories
{
    public interface IBackoffRepository : IQueueRepository<Queue<Command>>
	{
        Task<Queue<Queue<Command>>> Initialize(IEnumerable<IEnumerable<Command>> entity, Guid executionId);
        Task<Queue<Queue<Command>>> Refresh(Guid executionId);
        Task<Command> UpdateFirstAsync(Command entity, Guid executionId);
		CommandSettingsDto Settings { get; set; }
	}
}

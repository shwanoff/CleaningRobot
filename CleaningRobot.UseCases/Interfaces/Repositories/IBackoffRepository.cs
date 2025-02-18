using CleaningRobot.Entities.Entities;

namespace CleaningRobot.UseCases.Interfaces.Repositories
{
    public interface IBackoffRepository : IQueueRepository<Queue<Command>>
	{
        Task<Queue<Queue<Command>>> Initialize(IEnumerable<IEnumerable<Command>> entity, Guid executionId);
        Task<Queue<Queue<Command>>> Refresh(Guid executionId);
        Task<Command> UpdateFirstAsync(Command entity, Guid executionId);
	}
}

using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Interfaces;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
	public class GetCommandQueueQuery : IRequest<CommandQueueStatusDto>
	{
		public required Guid ExecutionId { get; set; }
	}

	public class GetCommandQueueQueryHandler(IRepository<Queue<Command>> commandRepository) : IRequestHandler<GetCommandQueueQuery, CommandQueueStatusDto>
	{
		private readonly IRepository<Queue<Command>> _commandRepository = commandRepository;

		public async Task<CommandQueueStatusDto> Handle(GetCommandQueueQuery request, CancellationToken cancellationToken = default)
		{
			var commandQueue = await _commandRepository.GetByIdAsync(request.ExecutionId);

			if (commandQueue == null)
			{
				throw new KeyNotFoundException($"Command queue for execution ID {request.ExecutionId} not found.");
			}

			return new CommandQueueStatusDto
			{
				ExecutionId = request.ExecutionId,
				Commands = (Queue<CommandStatusDto>)commandQueue.Select(c => new CommandStatusDto
				{
					Type = c.Type,
					EnergyConsumption = c.EnergyConsumption,
					IsCompleted = c.IsCompleted
				})
			};
		}
	}
}

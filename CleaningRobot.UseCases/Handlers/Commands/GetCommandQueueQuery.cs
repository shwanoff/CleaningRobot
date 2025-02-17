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
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request), "Request can not be null.");
			}

			var commandQueue = await _commandRepository.GetByIdAsync(request.ExecutionId);

			if (commandQueue == null)
			{
				throw new KeyNotFoundException($"Command queue for execution ID {request.ExecutionId} not found.");
			}

			return new CommandQueueStatusDto
			{
				IsCorrect = true,
				ExecutionId = request.ExecutionId,
				Commands = [.. commandQueue.Select(command => new CommandStatusDto
				{
					Type = command.Type,
					EnergyConsumption = command.EnergyConsumption,
					IsValid = command.IsValid,
					IsCompleted = command.IsCompleted,
					IsCorrect = true,
					ExecutionId = request.ExecutionId
				})]
			};
		}
	}
}

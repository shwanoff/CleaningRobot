using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Interfaces;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
    public class CreateCommandQueueCommand : IRequest<CommandQueueStatusDto>
    {
		public required Guid ExecutionId { get; set; }
		public required IEnumerable<CommandType> Commands { get; set; }
		public required IDictionary<CommandType, int> EnergyConsumptions { get; set; }
	}

	public class CreateCommandQueueCommandHandler(IRepository<Queue<Command>> commandRepository) : IRequestHandler<CreateCommandQueueCommand, CommandQueueStatusDto>
	{
		private readonly IRepository<Queue<Command>> _commandRepository = commandRepository;

		public async Task<CommandQueueStatusDto> Handle(CreateCommandQueueCommand request, CancellationToken cancellationToken = default)
		{
			if (request.Commands == null)
			{
				throw new ArgumentNullException(nameof(request.Commands), "Commands cannot be null");
			}

			if (request.EnergyConsumptions == null)
			{
				throw new ArgumentNullException(nameof(request.EnergyConsumptions), "Command energy consumptions cannot be null");
			}

			var commandQueue = await CreateCommandQueueAsync(request);

			var result = await _commandRepository.AddAsync(commandQueue, request.ExecutionId);

			if (result == null)
			{
				throw new InvalidOperationException("Command queue could not be created");
			}

			return new CommandQueueStatusDto
			{
				ExecutionId = request.ExecutionId,
				IsCorrect = true,
				Commands = [.. result.Select(command => new CommandStatusDto
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

		private static Task<Queue<Command>> CreateCommandQueueAsync(CreateCommandQueueCommand request)
		{
			var commandQueue = new Queue<Command>();

			foreach (var commandType in request.Commands)
			{
				var energyConsumption = request.EnergyConsumptions[commandType];

				var newCommand = new Command(commandType, energyConsumption);

				commandQueue.Enqueue(newCommand);
			}

			return Task.FromResult(commandQueue);
		}
	}
}

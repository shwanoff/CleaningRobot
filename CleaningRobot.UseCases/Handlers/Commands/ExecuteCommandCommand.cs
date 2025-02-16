using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Interfaces;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
	public class ExecuteCommandCommand : IRequest<ExecutionResultStatusDto<Command>>
	{
		public Guid ExecutionId { get; set; }
		public required Command Command { get; set; }
	}

	public class ExecuteCommandCommandHandler(IQueueRepository<Command> commandRepository) : IRequestHandler<ExecuteCommandCommand, ExecutionResultStatusDto<Command>>
	{
		private readonly IQueueRepository<Command> _commandRepository = commandRepository;

		public async Task<ExecutionResultStatusDto<Command>> Handle(ExecuteCommandCommand request, CancellationToken cancellationToken = default)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request), "Request cannot be null");
			}

			if (request.Command == null)
			{
				throw new ArgumentNullException(nameof(request.Command), "Command cannot be null");
			}

			if (request.Command.EnergyConsumption < 0)
			{
				throw new ArgumentException("The energy consumption of a command cannot be negative");
			}

			if (!request.Command.IsValid)
			{
				throw new ArgumentException($"Command '{request.Command}' is invalid");
			}

			request.Command.IsCompletedByCommand = true;
			await _commandRepository.UpdateFirstAsync(request.ExecutionId, request.Command);

			return new ExecutionResultStatusDto<Command>
			{
				IsCompleted = true,
				Result = request.Command,
				ExecutionId = request.ExecutionId
			};
		}
	}
}

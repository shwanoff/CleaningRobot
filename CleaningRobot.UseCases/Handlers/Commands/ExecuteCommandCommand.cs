using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Interfaces;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
	public class ExecuteCommandCommand : IRequest<ExecutionResultStatusDto<CommandStatusDto>>
	{
		public required Guid ExecutionId { get; set; }
		public required Command Command { get; set; }
	}

	public class ExecuteCommandCommandHandler(IQueueRepository<Command> commandRepository) : IRequestHandler<ExecuteCommandCommand, ExecutionResultStatusDto<CommandStatusDto>>
	{
		private readonly IQueueRepository<Command> _commandRepository = commandRepository;

		public async Task<ExecutionResultStatusDto<CommandStatusDto>> Handle(ExecuteCommandCommand request, CancellationToken cancellationToken = default)
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

			var newValues = new Dictionary<string, object>
			{
				{ nameof(Command.IsCompletedByCommand), true }
			};

			var result = await _commandRepository.UpdateFirstAsync(newValues, request.ExecutionId);

			if (result == null)
			{
				throw new InvalidOperationException($"Command '{request.Command}' could not be executed");
			}

			return new ExecutionResultStatusDto<CommandStatusDto>
			{
				Result = new CommandStatusDto
				{
					Type = result.Type,
					EnergyConsumption = result.EnergyConsumption,
					IsCorrect = true,
					IsValid = result.IsValid,
					IsCompleted = result.IsCompleted,
					ExecutionId = request.ExecutionId
				},
				IsCorrect = true,
				IsCompleted = true,
				ExecutionId = request.ExecutionId,
				State = ResultState.Ok
			};
		}
	}
}

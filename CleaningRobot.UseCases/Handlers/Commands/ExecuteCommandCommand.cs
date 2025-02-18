using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
	public class ExecuteCommandCommand : IRequest<ExecutionResultStatusDto<CommandStatusDto>>
	{
		public required Guid ExecutionId { get; set; }
		public required Command Command { get; set; }
		public required bool Backoff { get; set; }
	}

	public class ExecuteCommandCommandHandler(IQueueRepository<Command> commandRepository, IBackoffRepository backoffRepository) : IRequestHandler<ExecuteCommandCommand, ExecutionResultStatusDto<CommandStatusDto>>
	{
		private readonly IQueueRepository<Command> _commandRepository = commandRepository;
		private readonly IBackoffRepository _backoffRepository = backoffRepository;

		public async Task<ExecutionResultStatusDto<CommandStatusDto>> Handle(ExecuteCommandCommand request, CancellationToken cancellationToken = default)
		{
			request.NotNull();
			request.Command.NotNull();
			request.Command.EnergyConsumption.IsPositive();
			request.Command.IsValid.IsTrue();

			request.Command.IsCompletedByCommand = true;

			Command result;
			if (request.Backoff)
			{
				result = await _backoffRepository
					.UpdateFirstAsync(request.Command, request.ExecutionId)
					.NotNull();
			}
			else
			{
				result = await _commandRepository
					.UpdateFirstAsync(request.Command, request.ExecutionId)
					.NotNull();
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

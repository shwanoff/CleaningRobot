using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Interfaces;
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

	public class ExecuteCommandCommandHandler(IQueueRepository<Command> commandRepository, IBackoffRepository backoffRepository, ILogAdapter logAdapter) : IRequestHandler<ExecuteCommandCommand, ExecutionResultStatusDto<CommandStatusDto>>
	{
		private readonly IQueueRepository<Command> _commandRepository = commandRepository;
		private readonly IBackoffRepository _backoffRepository = backoffRepository;
		private readonly ILogAdapter _logAdapter = logAdapter;

		public async Task<ExecutionResultStatusDto<CommandStatusDto>> Handle(ExecuteCommandCommand request, CancellationToken cancellationToken = default)
		{
			try
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

				await _logAdapter.TraceAsync($"Command executed. Command state {request.Command}" + (request.Backoff ? "BACKOFF" : ""), request.ExecutionId);

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
			catch (Exception ex)
			{
				await _logAdapter.ErrorAsync(ex.Message, nameof(ExecuteCommandCommandHandler), request?.ExecutionId ?? Guid.Empty, ex);
				throw;
			}
		}
	}
}

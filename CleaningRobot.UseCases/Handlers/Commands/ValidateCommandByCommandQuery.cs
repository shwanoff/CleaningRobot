using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
    public class ValidateCommandByCommandQuery : IRequest<ValidationResultStatusDto>
    {
		public required Guid ExecutionId { get; set; }
		public required Command Command { get; set; }
		public required bool Backoff { get; set; }
	}

	public class ValidateCommandByCommandQueryHandler(IQueueRepository<Command> commandRepository, IBackoffRepository backoffRepository, ILogAdapter logAdapter) : IRequestHandler<ValidateCommandByCommandQuery, ValidationResultStatusDto>
	{
		private readonly IQueueRepository<Command> _commandRepository = commandRepository;
		private readonly IBackoffRepository _backoffRepository = backoffRepository;
		private readonly ILogAdapter _logAdapter = logAdapter;

		public async Task<ValidationResultStatusDto> Handle(ValidateCommandByCommandQuery request, CancellationToken cancellationToken)
		{
			try
			{
				if (request == null)
				{
					return new ValidationResultStatusDto
					{
						IsCorrect = false,
						IsValid = false,
						Error = "Request cannot be null",
						ExecutionId = Guid.Empty,
						State = ResultState.ValidationError
					};
				}

				if (request.Command == null)
				{
					return new ValidationResultStatusDto
					{
						IsCorrect = false,
						IsValid = false,
						Error = "Command cannot be null",
						ExecutionId = request.ExecutionId,
						State = ResultState.ValidationError
					};
				}

				if (request.Command.EnergyConsumption < 0)
				{
					return new ValidationResultStatusDto
					{
						IsCorrect = false,
						IsValid = false,
						Error = "The energy consumption of a command cannot be negative",
						ExecutionId = request.ExecutionId,
						State = ResultState.ValidationError
					};
				}

				request.Command.IsValidatedByCommand = true;

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

				if (result == null)
				{
					return new ValidationResultStatusDto
					{
						IsCorrect = false,
						IsValid = false,
						Error = "Cannot update the command after validation",
						ExecutionId = request.ExecutionId,
						State = ResultState.ValidationError
					};
				}

				await _logAdapter.TraceAsync($"Command validated. Command state {request.Command}" + (request.Backoff ? "BACKOFF" : ""), request.ExecutionId);

				return new ValidationResultStatusDto
				{
					IsCorrect = true,
					IsValid = true,
					ExecutionId = request.ExecutionId,
					State = ResultState.Ok
				};
			}
			catch (Exception ex)
			{
				await _logAdapter.ErrorAsync(ex.Message, nameof(ValidateCommandByCommandQueryHandler), request?.ExecutionId ?? Guid.Empty, ex);
				throw;
			}
		}
	}
}

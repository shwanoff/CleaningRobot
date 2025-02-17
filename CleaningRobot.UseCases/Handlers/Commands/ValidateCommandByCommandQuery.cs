using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Interfaces;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Commands
{
    public class ValidateCommandByCommandQuery : IRequest<ValidationResultStatusDto>
    {
		public required Guid ExecutionId { get; set; }
		public required Command Command { get; set; }
	}

	public class ValidateCommandByCommandQueryHandler(IQueueRepository<Command> commandRepository) : IRequestHandler<ValidateCommandByCommandQuery, ValidationResultStatusDto>
	{
		private readonly IQueueRepository<Command> _commandRepository = commandRepository;

		public async Task<ValidationResultStatusDto> Handle(ValidateCommandByCommandQuery request, CancellationToken cancellationToken)
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

			var newValues = new Dictionary<string, object>
			{
				{ nameof(Command.IsValidatedByCommand), true }
			};

			var result = await _commandRepository.UpdateFirstAsync(newValues, request.ExecutionId);

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

			return new ValidationResultStatusDto
			{
				IsCorrect = true,
				IsValid = true,
				ExecutionId = request.ExecutionId,
				State = ResultState.Ok
			};
		}
	}
}

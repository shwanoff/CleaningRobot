using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
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
					IsValid = false,
					Error = "Request cannot be null"
				};
			}

			if (request.Command == null)
			{
				return new ValidationResultStatusDto 
				{
					IsValid = false,
					Error = "Command cannot be null",
					ExecutionId = request.ExecutionId
				};
			}
			
			if (request.Command.EnergyConsumption < 0)
			{
				return new ValidationResultStatusDto
				{
					IsValid = false,
					Error = "The energy consumption of a command cannot be negative",
					ExecutionId = request.ExecutionId
				};
			}

			request.Command.IsValidatedByCommand = true;
			await _commandRepository.UpdateFirstAsync(request.ExecutionId, request.Command);

			return new ValidationResultStatusDto
			{
				IsValid = true,
				ExecutionId = request.ExecutionId
			};
		}
	}
}

using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Interfaces;
using MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CleaningRobot.UseCases.Handlers.Robots
{
	public class ValidateCommandByRobotQuery : IRequest<ValidationResultStatusDto>
	{
		public required Guid ExecutionId { get; set; }
		public required Command Command { get; set; }
	}

	public class ValidateCommandByRobotQueryHandler(IRepository<Robot> robotRepository) : IRequestHandler<ValidateCommandByRobotQuery, ValidationResultStatusDto>
	{
		private readonly IRepository<Robot> _robotRepository = robotRepository;

		public async Task<ValidationResultStatusDto> Handle(ValidateCommandByRobotQuery request, CancellationToken cancellationToken)
		{
			if (request == null)
			{
				return new ValidationResultStatusDto
				{
					IsCompleted = false,
					Error = "Request cannot be null"
				};
			}

			if (request.Command == null)
			{
				return new ValidationResultStatusDto
				{
					IsCompleted = false,
					Error = "Command cannot be null",
					ExecutionId = request.ExecutionId
				};
			}

			if (request.Command.EnergyConsumption < 0)
			{
				return new ValidationResultStatusDto
				{
					IsCompleted = false,
					Error = "The energy consumption of a command cannot be negative",
					ExecutionId = request.ExecutionId
				};
			}

			var robot = await _robotRepository.GetByIdAsync(request.ExecutionId);
			if (robot == null)
			{
				return new ValidationResultStatusDto
				{
					IsCompleted = false,
					Error = $"Robot for execution ID {request.ExecutionId} not found.",
					ExecutionId = request.ExecutionId
				};
			}


			if (!IsValidCommandForRobot(request.Command, robot, out string? error))
			{
				return new ValidationResultStatusDto
				{
					IsCompleted = false,
					Error = error,
					ExecutionId = request.ExecutionId
				};
			}

			request.Command.IsValidatedByRobot = true;

			return new ValidationResultStatusDto
			{
				IsCompleted = true,
				ExecutionId = request.ExecutionId
			};
		}

		private bool IsValidCommandForRobot(Command command, Robot robot, out string? error)
		{
			error = null;

			if (robot.Battery < command.EnergyConsumption)
			{
				error = $"The battery level is too low. The battery level is {robot.Battery} and the command requires {command.EnergyConsumption}";
				return false;
			}

			return true;
		}
	}
}

using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Maps
{
	public class ValidateCommandByMapQuery : IRequest<ValidationResultStatusDto>
	{
		public required Guid ExecutionId { get; set; }
		public required Command Command { get; set; }
	}

	public class ValidateCommandByMapQueryHandler(IRepository<Map> mapRepository, IRepository<Robot> robotRepository, IQueueRepository<Command> commandRepository) : IRequestHandler<ValidateCommandByMapQuery, ValidationResultStatusDto>
	{
		private readonly IRepository<Map> _mapRepository = mapRepository;
		private readonly IRepository<Robot> _robotRepository = robotRepository;
		private readonly IQueueRepository<Command> _commandRepository = commandRepository;

		public async Task<ValidationResultStatusDto> Handle(ValidateCommandByMapQuery request, CancellationToken cancellationToken)
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

			var map = await _mapRepository.GetByIdAsync(request.ExecutionId);

			if (map == null)
			{
				return new ValidationResultStatusDto
				{
					IsCorrect = false,
					IsValid = false,
					Error = $"Map for execution ID {request.ExecutionId} not found.",
					ExecutionId = request.ExecutionId,
					State = ResultState.ValidationError
				};
			}

			var robot = await _robotRepository.GetByIdAsync(request.ExecutionId);

			if (robot == null)
			{
				return new ValidationResultStatusDto
				{
					IsCorrect = false,
					IsValid = false,
					Error = $"Robot for execution ID {request.ExecutionId} not found.",
					ExecutionId = request.ExecutionId,
					State = ResultState.ValidationError
				};
			}

			if (!IsValidCommandForMap(request.Command, map, robot, out string? error))
			{
				return new ValidationResultStatusDto
				{
					IsCorrect = false,
					IsValid = false,
					Error = error,
					ExecutionId = request.ExecutionId,
					State = ResultState.BackOff
				};
			}

			var newValues = new Dictionary<string, object>
			{
				{ nameof(Command.IsValidatedByMap), true }
			};

			var result = await _commandRepository.UpdateFirstAsync(newValues, request.ExecutionId);

			if (result == null)
			{
				return new ValidationResultStatusDto
				{
					IsCorrect = false,
					IsValid = false,
					ExecutionId = request.ExecutionId,
					Error = "Cannot update the command after validation by map",
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

		private static bool IsValidCommandForMap(Command command, Map map, Robot robot, out string? error)
		{
			error = null;

			switch (command.Type)
			{
				case CommandType.TurnLeft:
				case CommandType.TurnRight:
				case CommandType.Clean:
					return true;
				case CommandType.Advance:
				case CommandType.Back:
					return IsNextCellAwailable(command, map, robot, out error);
				default:
					error = $"Type '{command.Type}' is invalid";
					return false;
			}
		}

		private static bool IsNextCellAwailable(Command command, Map map, Robot robot, out string? error)
		{
			var nextPositon = PositionHelper.GetNextPosition(robot.Position, command.Type);

			if (!PositionHelper.IsCellAvailable(map, nextPositon, out error))
			{
				return false;
			}

			return true;
		}

		
	}
}

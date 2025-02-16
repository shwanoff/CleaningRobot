using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Dto.Output;
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

	public class ValidateCommandByMapQueryHandler(IRepository<Map> mapRepository, IRepository<Robot> robotRepository) : IRequestHandler<ValidateCommandByMapQuery, ValidationResultStatusDto>
	{
		private readonly IRepository<Map> _mapRepository = mapRepository;
		private readonly IRepository<Robot> _robotRepository = robotRepository;

		public async Task<ValidationResultStatusDto> Handle(ValidateCommandByMapQuery request, CancellationToken cancellationToken)
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

			var map = await _mapRepository.GetByIdAsync(request.ExecutionId);
			if (map == null)
			{
				return new ValidationResultStatusDto
				{
					IsCompleted = false,
					Error = $"Map for execution ID {request.ExecutionId} not found.",
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

			if (!IsValidCommandForMap(request.Command, map, robot, out string? error))
			{
				return new ValidationResultStatusDto
				{
					IsCompleted = false,
					Error = error,
					ExecutionId = request.ExecutionId
				};
			}

			request.Command.IsValidatedByMap = true;

			return new ValidationResultStatusDto
			{
				IsCompleted = true,
				ExecutionId = request.ExecutionId
			};
		}

		private bool IsValidCommandForMap(Command command, Map map, Robot robot, out string? error)
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
			error = null;
			var robotPosition = robot.Position;
			var nextPositon = PositionHelper.GetNextPosition(robotPosition, command.Type);

			if (nextPositon.X < 0 || nextPositon.X >= map.Width || nextPositon.Y < 0 || nextPositon.Y >= map.Height)
			{
				error = "The robot cannot move outside the map";
				return false;
			}

			var nextCell = map.Cells[nextPositon.X, nextPositon.Y];

			if (nextCell.Type == CellType.Wall)
			{
				error = "The robot cannot move into a wall";
				return false;
			}

			if (nextCell.Type == CellType.Column)
			{
				error = "The robot cannot move into a column";
				return false;
			}

			return true;
		}
	}
}

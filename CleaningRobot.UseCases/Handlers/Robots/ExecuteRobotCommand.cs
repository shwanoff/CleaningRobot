using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Interfaces;
using MediatR;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Helpers;

namespace CleaningRobot.UseCases.Handlers.Robots
{
	public class ExecuteRobotCommand : IRequest<ExecutionResultStatusDto<Robot>>
	{
		public Guid ExecutionId { get; set; }
		public required Command Command { get; set; }
	}

	public class ExecuteRobotCommandHandler(IRepository<Robot> robotRepository) : IRequestHandler<ExecuteRobotCommand, ExecutionResultStatusDto<Robot>>
	{
		private readonly IRepository<Robot> _robotRepository = robotRepository;

		public async Task<ExecutionResultStatusDto<Robot>> Handle(ExecuteRobotCommand request, CancellationToken cancellationToken = default)
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

			var robot = await _robotRepository.GetByIdAsync(request.ExecutionId);

			if (robot == null)
			{
				throw new KeyNotFoundException($"Robot for execution ID {request.ExecutionId} not found.");
			}

			Execute(request.Command, robot);
			request.Command.IsCompletedByRobot = true;
			await _robotRepository.UpdateAsync(request.ExecutionId, robot);

			return new ExecutionResultStatusDto<Robot>
			{
				IsCompleted = true,
				Result = robot,
				ExecutionId = request.ExecutionId
			};
		}

		private void Execute(Command command, Robot robot)
		{
			switch (command.Type)
			{
				case CommandType.TurnLeft:
					TurnLeft(robot, command.EnergyConsumption);
					break;
				case CommandType.TurnRight:
					TurnRight(robot, command.EnergyConsumption);
					break;
				case CommandType.Advance:
					Advance(robot, command.EnergyConsumption);
					break;
				case CommandType.Back:
					Back(robot, command.EnergyConsumption);
					break;
				case CommandType.Clean:
					Clean(robot, command.EnergyConsumption);
					break;
				default:
					throw new ArgumentException($"Type '{command}' is invalid. Valid values are: TL, TR, A, B, C, TurnLeft, Turn Right, Advance, Back, Clean");
			}
		}

		private void TurnLeft(Robot robot, int energy)
		{
			robot.Position.Facing = robot.Position.Facing switch
			{
				Facing.North => Facing.West,
				Facing.West => Facing.South,
				Facing.South => Facing.East,
				Facing.East => Facing.North,
				_ => throw new ArgumentException($"Facing '{robot.Position.Facing}' is invalid")
			};

			ConsumeBattery(robot, energy);
		}

		private void TurnRight(Robot robot, int energy)
		{
			robot.Position.Facing = robot.Position.Facing switch
			{
				Facing.North => Facing.East,
				Facing.East => Facing.South,
				Facing.South => Facing.West,
				Facing.West => Facing.North,
				_ => throw new ArgumentException($"Facing '{robot.Position.Facing}' is invalid")
			};

			ConsumeBattery(robot, energy);
		}

		private void Advance(Robot robot, int energy)
		{
			var nextPositon = PositionHelper.GetNextPosition(robot.Position, CommandType.Advance);
			Move(robot, nextPositon);

			ConsumeBattery(robot, energy);
		}

		private void Back(Robot robot, int energy)
		{
			var nextPositon = PositionHelper.GetNextPosition(robot.Position, CommandType.Back);
			Move(robot, nextPositon);

			ConsumeBattery(robot, energy);
		}

		private void Clean(Robot robot, int energy)
		{
			ConsumeBattery(robot, energy);
		}

		private void ConsumeBattery(Robot robot, int energy)
		{
			robot.Battery -= energy;
		}

		private void Move(Robot robot, Position newPosition)
		{
			robot.Position.X = newPosition.X;
			robot.Position.Y = newPosition.Y;
		}
	}
}

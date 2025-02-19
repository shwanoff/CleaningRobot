using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using MediatR;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Interfaces.Repositories;
using CleaningRobot.Entities.Interfaces;

namespace CleaningRobot.UseCases.Handlers.Robots
{
	public class ExecuteRobotCommand : IRequest<ExecutionResultStatusDto<RobotStatusDto>>
	{
		public required Guid ExecutionId { get; set; }
		public required Command Command { get; set; }
	}

	public class ExecuteRobotCommandHandler(IRepository<Robot> robotRepository, IQueueRepository<Command> commandRepository, ILogAdapter logAdapter) : IRequestHandler<ExecuteRobotCommand, ExecutionResultStatusDto<RobotStatusDto>>
	{
		private readonly IRepository<Robot> _robotRepository = robotRepository;
		private readonly IQueueRepository<Command> _commandRepository = commandRepository;
		private readonly ILogAdapter _logAdapter = logAdapter;

		public async Task<ExecutionResultStatusDto<RobotStatusDto>> Handle(ExecuteRobotCommand request, CancellationToken cancellationToken = default)
		{
			try
			{
				request.NotNull();
				request.Command.NotNull();
				request.Command.EnergyConsumption.IsPositive();
				request.Command.IsValid.IsTrue();

				var robot = await _robotRepository
					.GetByIdAsync(request.ExecutionId)
					.NotNull();

				Execute(request.Command, robot);

				request.Command.IsCompletedByRobot = true;

				var commandResult = await _commandRepository
					.UpdateFirstAsync(request.Command, request.ExecutionId)
					.NotNull();

				var result = await _robotRepository
					.UpdateAsync(robot, request.ExecutionId)
					.NotNull();

				await _logAdapter.TraceAsync($"Robot executed command {request.Command}. Robot state {robot}", request.ExecutionId);

				return new ExecutionResultStatusDto<RobotStatusDto>
				{
					Result = new RobotStatusDto
					{
						X = robot.Position.X,
						Y = robot.Position.Y,
						Facing = robot.Position.Facing,
						Battery = robot.Battery,
						IsCorrect = true,
						ExecutionId = request.ExecutionId
					},
					IsCorrect = true,
					ExecutionId = request.ExecutionId,
					IsCompleted = true,
					State = ResultState.Ok
				};
			}
			catch (Exception ex)
			{
				await _logAdapter.ErrorAsync(ex.Message, nameof(ExecuteRobotCommandHandler), request?.ExecutionId ?? Guid.Empty, ex);
				throw;
			}
		}

		private static void Execute(Command command, Robot robot)
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

		private static void TurnLeft(Robot robot, int energy)
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

		private static void TurnRight(Robot robot, int energy)
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

		private static void Advance(Robot robot, int energy)
		{
			var nextPositon = PositionHelper.GetNextPosition(robot.Position, CommandType.Advance);

			Move(robot, nextPositon);

			ConsumeBattery(robot, energy);
		}

		private static void Back(Robot robot, int energy)
		{
			var nextPositon = PositionHelper.GetNextPosition(robot.Position, CommandType.Back);
			
			Move(robot, nextPositon);

			ConsumeBattery(robot, energy);
		}

		private static void Clean(Robot robot, int energy)
		{
			ConsumeBattery(robot, energy);
		}

		private static void ConsumeBattery(Robot robot, int energy)
		{
			robot.Battery -= energy;
		}

		private static void Move(Robot robot, Position newPosition)
		{
			robot.Position.X = newPosition.X;
			robot.Position.Y = newPosition.Y;
		}
	}
}

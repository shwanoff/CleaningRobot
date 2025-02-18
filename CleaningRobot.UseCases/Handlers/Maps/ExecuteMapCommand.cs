using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using MediatR;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Enums;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;

namespace CleaningRobot.UseCases.Handlers.Maps
{
	public class ExecuteMapCommand : IRequest<ExecutionResultStatusDto<MapStatusDto>>
	{
		public required Guid ExecutionId { get; set; }
		public required Command Command { get; set; }
	}

	public class ExecuteMapCommandHandler(IRepository<Map> mapRepository, IRepository<Robot> robotRepository, IQueueRepository<Command> commandRepository) : IRequestHandler<ExecuteMapCommand, ExecutionResultStatusDto<MapStatusDto>>
	{
		private readonly IRepository<Map> _mapRepository = mapRepository;
		private readonly IRepository<Robot> _robotRepository = robotRepository;
		private readonly IQueueRepository<Command> _commandRepository = commandRepository;

		public async Task<ExecutionResultStatusDto<MapStatusDto>> Handle(ExecuteMapCommand request, CancellationToken cancellationToken = default)
		{
			request.NotNull();
			request.Command.NotNull();
			request.Command.EnergyConsumption.IsPositive();
			request.Command.IsValid.IsTrue();

			var map = await _mapRepository
				.GetByIdAsync(request.ExecutionId)
				.NotNull();

			var robot = await _robotRepository
				.GetByIdAsync(request.ExecutionId)
				.NotNull();

			Execute(request.Command, map, robot);

			request.Command.IsCompletedByMap = true;

			var commandResult = await _commandRepository
				.UpdateFirstAsync(request.Command, request.ExecutionId)
				.NotNull();

			var result = await _mapRepository
				.UpdateAsync(map, request.ExecutionId)
				.NotNull();

			return new ExecutionResultStatusDto<MapStatusDto>
			{
				Result = new MapStatusDto
				{
					ExecutionId = request.ExecutionId,
					Width = result.Width,
					Height = result.Height,
					IsCorrect = true,
					Cells = [.. result.Cells.Cast<Cell>()
					.Select(c => new CellStatusDto
					{
						X = c.Position.X,
						Y = c.Position.Y,
						Type = c.Type,
						State = c.State,
						ExecutionId = request.ExecutionId,
						IsCorrect = true,
					})]
				},
				IsCorrect = true,
				IsCompleted = true,
				ExecutionId = request.ExecutionId,
				State = ResultState.Ok
			};
		}

		private static void Execute(Command command, Map map, Robot robot)
		{
			switch (command.Type)
			{
				case CommandType.Clean:
					Clean(map, robot);
					break;
				case CommandType.Advance:
				case CommandType.Back:
					Move(map, robot);
					break;
			}

		}

		private static void Clean(Map map, Robot robot)
		{
			var position = robot.Position;
			map.Cells[position.Y, position.X].State = CellState.Cleaned;
		}

		private static void Move(Map map, Robot robot)
		{
			var position = robot.Position;
			var cell = map.Cells[position.Y, position.X];
			if (cell.State == CellState.NotVisited)
			{
				cell.State = CellState.Visited;
			}
		}
	}
}

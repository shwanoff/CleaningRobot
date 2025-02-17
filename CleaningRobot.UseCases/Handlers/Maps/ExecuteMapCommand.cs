using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Interfaces;
using MediatR;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Repositories;

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

			var map = await _mapRepository.GetByIdAsync(request.ExecutionId);

			if (map == null)
			{
				throw new KeyNotFoundException($"Map for execution ID {request.ExecutionId} not found.");
			}

			var robot = await _robotRepository.GetByIdAsync(request.ExecutionId);

			if (robot == null)
			{
				throw new KeyNotFoundException($"Robot for execution ID {request.ExecutionId} not found.");
			}

			Execute(request.Command, map, robot);

			var newValuesCommand = new Dictionary<string, object>
			{
				{ nameof(Command.IsCompletedByMap), true }
			};

			var commandResult = await _commandRepository.UpdateFirstAsync(newValuesCommand, request.ExecutionId);

			if (commandResult == null)
			{
				throw new InvalidOperationException($"Command '{request.Command}' could not be executed");
			}

			var result = await _mapRepository.UpdateAsync(map, request.ExecutionId);

			if (result == null)
			{
				throw new InvalidOperationException($"Map for execution ID {request.ExecutionId} could not be updated");
			}

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
						IsCorrect = true
					})]
				},
				IsCorrect = true,
				IsCompleted = true,
				ExecutionId = request.ExecutionId	
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
			map.Cells[position.X, position.Y].State = CellState.Cleaned;
		}

		private static void Move(Map map, Robot robot)
		{
			var position = robot.Position;
			map.Cells[position.X, position.Y].State = CellState.Visited;
		}
	}
}

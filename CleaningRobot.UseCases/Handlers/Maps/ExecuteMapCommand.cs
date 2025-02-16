using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Interfaces;
using MediatR;
using CleaningRobot.Entities.Enums;

namespace CleaningRobot.UseCases.Handlers.Maps
{
	public class ExecuteMapCommand : IRequest<ExecutionResultStatusDto<Map>>
	{
		public required Guid ExecutionId { get; set; }
		public required Command Command { get; set; }
	}

	public class ExecuteMapCommandHandler(IRepository<Map> mapRepository, IRepository<Robot> robotRepository) : IRequestHandler<ExecuteMapCommand, ExecutionResultStatusDto<Map>>
	{
		private readonly IRepository<Map> _mapRepository = mapRepository;
		private readonly IRepository<Robot> _robotRepository = robotRepository;

		public async Task<ExecutionResultStatusDto<Map>> Handle(ExecuteMapCommand request, CancellationToken cancellationToken = default)
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
			request.Command.IsCompletedByMap = true;
			await _mapRepository.UpdateAsync(request.ExecutionId, map);

			return new ExecutionResultStatusDto<Map>
			{
				IsCompleted = true,
				Result = map,
				ExecutionId = request.ExecutionId	
			};
		}

		private void Execute(Command command, Map map, Robot robot)
		{
			switch (command.Type)
			{
				case CommandType.Clean:
					Clean(map, robot);
					break;
				case CommandType.Advance:
				case CommandType.Back:
					Visit(map, robot);
					break;
			}

		}

		private void Clean(Map map, Robot robot)
		{
			var position = robot.Position;
			map.Cells[position.X, position.Y].State = CellState.Cleaned;
		}

		private void Visit(Map map, Robot robot)
		{
			var position = robot.Position;
			map.Cells[position.X, position.Y].State = CellState.Visited;
		}
	}
}

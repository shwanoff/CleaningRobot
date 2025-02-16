using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Interfaces;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Robots
{
	public class GetRobotQuery : IRequest<RobotStatusDto>
	{
		public required Guid ExecutionId { get; set; }
	}

	public class GetRobotQueryHandler(IRepository<Robot> robotRepository) : IRequestHandler<GetRobotQuery, RobotStatusDto>
	{
		private readonly IRepository<Robot> _robotRepository = robotRepository;

		public async Task<RobotStatusDto> Handle(GetRobotQuery request, CancellationToken cancellationToken = default)
		{
			var robot = await _robotRepository.GetByIdAsync(request.ExecutionId);

			if (robot == null)
			{
				throw new KeyNotFoundException($"Robot for execution ID {request.ExecutionId} not found.");
			}

			return new RobotStatusDto
			{
				ExecutionId = request.ExecutionId,
				X = robot.Position.X,
				Y = robot.Position.Y,
				Facing = robot.Position.Facing,
				Battery = robot.Battery
			};
		}
	}
}

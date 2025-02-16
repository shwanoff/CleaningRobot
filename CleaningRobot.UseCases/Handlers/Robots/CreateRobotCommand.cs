using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Extensions;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Interfaces;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Robots
{
	public class CreateRobotCommand : IRequest<RobotStatusDto>
	{
		public required Guid ExecutionId { get; set; }
		public required RobotPosition Position { get; set; }
		public required int Battery { get; set; }
	}

	public class CreateRobotCommandHandler(IRepository<Robot> robotRepository) : IRequestHandler<CreateRobotCommand, RobotStatusDto>
	{
		private readonly IRepository<Robot> _robotRepository = robotRepository;

		public async Task<RobotStatusDto> Handle(CreateRobotCommand request, CancellationToken cancellationToken = default)
		{
			var robot = new Robot(request.Position, request.Battery);

			await _robotRepository.AddAsync(request.ExecutionId, robot);

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

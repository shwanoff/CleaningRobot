using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Robots
{
	public class GetRobotQuery : IRequest<RobotStatusDto>
	{
		public required Guid ExecutionId { get; set; }
	}

	public class GetRobotQueryHandler(IRepository<Robot> robotRepository, ILogAdapter logAdapter) : IRequestHandler<GetRobotQuery, RobotStatusDto>
	{
		private readonly IRepository<Robot> _robotRepository = robotRepository;
		private readonly ILogAdapter _logAdapter = logAdapter;

		public async Task<RobotStatusDto> Handle(GetRobotQuery request, CancellationToken cancellationToken = default)
		{
			try
			{
				request.NotNull();

				var robot = await _robotRepository
					.GetByIdAsync(request.ExecutionId)
					.NotNull();

				await _logAdapter.TraceAsync($"Robot state {robot}", request.ExecutionId);

				return new RobotStatusDto
				{
					ExecutionId = request.ExecutionId,
					X = robot.Position.X,
					Y = robot.Position.Y,
					Facing = robot.Position.Facing,
					Battery = robot.Battery,
					IsCorrect = true
				};
			}
			catch (Exception ex)
			{
				await _logAdapter.ErrorAsync(ex.Message, nameof(GetRobotQueryHandler), request?.ExecutionId ?? Guid.Empty, ex);
				throw;
			}
		}
	}
}

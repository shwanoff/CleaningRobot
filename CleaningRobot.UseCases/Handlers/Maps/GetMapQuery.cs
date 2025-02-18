using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Maps
{
	public class GetMapQuery : IRequest<MapStatusDto>
	{
		public required Guid ExecutionId { get; set; }
	}

	public class GetMapQueryHandler(IRepository<Map> mapRepository) : IRequestHandler<GetMapQuery, MapStatusDto>
	{
		private readonly IRepository<Map> _mapRepository = mapRepository;

		public async Task<MapStatusDto> Handle(GetMapQuery request, CancellationToken cancellationToken = default)
		{
			request.NotNull();

			var map = await _mapRepository
				.GetByIdAsync(request.ExecutionId)
				.NotNull();

			return new MapStatusDto
			{
				ExecutionId = request.ExecutionId,
				Width = map.Width,
				Height = map.Height,
				IsCorrect = true,
				Cells = [.. map.Cells.Cast<Cell>()
					.Select(c => new CellStatusDto
					{
						X = c.Position.X,
						Y = c.Position.Y,
						Type = c.Type,
						State = c.State,
						ExecutionId = request.ExecutionId,
						IsCorrect = true
					})]
			};
		}
	}
}

using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Interfaces;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Maps
{
	public class GetMapQuery : IRequest<MapStatusDto>
	{
		public Guid ExecutionId { get; set; }
	}

	public class GetMapQueryHandler(IRepository<Map> mapRepository) : IRequestHandler<GetMapQuery, MapStatusDto>
	{
		private readonly IRepository<Map> _mapRepository = mapRepository;

		public async Task<MapStatusDto> Handle(GetMapQuery request, CancellationToken cancellationToken = default)
		{
			var map = await _mapRepository.GetByIdAsync(request.ExecutionId);

			if (map == null)
			{
				throw new KeyNotFoundException($"Map for execution ID {request.ExecutionId} not found.");
			}

			return new MapStatusDto
			{
				ExecutionId = request.ExecutionId,
				Width = map.Width,
				Height = map.Height,
				Cells = [.. map.Cells.Cast<Cell>()
					.Select(c => new CellStatusDto
					{
						X = c.Position.X,
						Y = c.Position.Y,
						Type = c.Type,
						State = c.State 
					})]
			};
		}
	}
}

﻿using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Extensions;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Maps
{
	public class CreateMapCommand : IRequest<MapStatusDto>
	{
		public required Guid ExecutionId { get; set; }
		public required string[][] MapData { get; set; }
	}

	public class CreateMapCommandHandler(IRepository<Map> mapRepository) : IRequestHandler<CreateMapCommand, MapStatusDto>
	{
		private readonly IRepository<Map> _mapRepository = mapRepository;

		public async Task<MapStatusDto> Handle(CreateMapCommand request, CancellationToken cancellationToken = default)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request), "Request cannot be null");
			}

			if (request.MapData == null)
			{
				throw new ArgumentNullException(nameof(request.MapData), "Map data cannot be null");
			}

			var map = await CreateMapAsync(request);

			await _mapRepository.AddAsync(request.ExecutionId, map);

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

		private static Task<Map> CreateMapAsync(CreateMapCommand request)
		{
			var cells = MapHelper.ConvertToRectangularArray(request.MapData);
			var map = new Map(cells);
			return Task.FromResult(map);
		}
	}
}

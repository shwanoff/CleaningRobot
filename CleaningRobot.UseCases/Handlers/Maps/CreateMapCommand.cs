﻿using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Interfaces;
using CleaningRobot.UseCases.Dto.Output;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Repositories;
using MediatR;

namespace CleaningRobot.UseCases.Handlers.Maps
{
	public class CreateMapCommand : IRequest<MapStatusDto>
	{
		public required Guid ExecutionId { get; set; }
		public required string[][] MapData { get; set; }
	}

	public class CreateMapCommandHandler(IRepository<Map> mapRepository, ILogAdapter logAdapter) : IRequestHandler<CreateMapCommand, MapStatusDto>
	{
		private readonly IRepository<Map> _mapRepository = mapRepository;
		private readonly ILogAdapter _logAdapter = logAdapter;

		public async Task<MapStatusDto> Handle(CreateMapCommand request, CancellationToken cancellationToken = default)
		{
			try
			{
				request.NotNull();
				request.MapData.NotNull();

				var map = await CreateMapAsync(request);

				var result = await _mapRepository
					.AddAsync(map, request.ExecutionId)
					.NotNull();

				await _logAdapter.TraceAsync($"Map created. Map state {map}", request.ExecutionId);
				await _logAdapter.DebugAsync(map.Draw(), request.ExecutionId);

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
			catch (Exception ex)
			{
				await _logAdapter.ErrorAsync(ex.Message, nameof(CreateMapCommandHandler), request?.ExecutionId ?? Guid.Empty, ex);
				throw;
			}
		}

		#region Private methods
		private static Task<Map> CreateMapAsync(CreateMapCommand request)
		{
			var cells = MapHelper.ConvertToRectangularArray(request.MapData);
			var map = new Map(cells);
			return Task.FromResult(map);
		}
		#endregion
	}
}

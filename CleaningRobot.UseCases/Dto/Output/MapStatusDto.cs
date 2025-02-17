﻿using CleaningRobot.UseCases.Dto.Base;

namespace CleaningRobot.UseCases.Dto.Output
{
    public class MapStatusDto : StatusDtoBase
	{
		public required IReadOnlyList<CellStatusDto> Cells { get; set; }
		public required int Width { get; set; }
		public required int Height { get; set; }

		#if DEBUG
		public override string ToString()
		{
			var success = IsCorrect ? "Success" : "Failed";
			return $"[{success}] {Width}x{Height} {string.Join(',', Cells)}";
		}
		#endif
	}
}

using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Extensions;
using CleaningRobot.UseCases.Dto;
using CleaningRobot.UseCases.Interfaces.Controllers;
using CleaningRobot.UseCases.Interfaces.Operations;

namespace CleaningRobot.UseCases.Controllers
{
	public class MapController : IMapController, IMapOperation
	{
		private readonly IRobotController _robotController;

		private Map _map;

		public void Create(string[][] map)
		{
			_map = new Map(ConvertToRectangularArray(map));
		}

		public IEnumerable<CellStatusDto> GetAllCellStatuses()
		{
			for (int y = 0; y < _map.Height; y++)
			{
				for (int x = 0; x < _map.Width; x++)
				{
					yield return new CellStatusDto
					{
						X = y,
						Y = x,
						Type = _map.Cells[y, x].Type,
						State = _map.Cells[y, x].State
					};
				}
			}
		}

		public void Update(Command command)
		{
			if (ValidateCommand(command, out string? error))
			{
				var cell = GetCurrentRobotCell();
				switch (command.CommandType)
				{
					case CommandType.Clean:
						Clean(cell);
						break;
					case CommandType.Advance:
					case CommandType.Back:
						Visit(cell);
						break;
				}
			}
			else
			{
				throw new ArgumentException(error);
			}
		}

		public bool ValidateCommand(Command command, out string? error)
		{
			error = null;

			ArgumentNullException.ThrowIfNull(command);

			switch (command.CommandType)
			{
				case CommandType.TurnLeft:
				case CommandType.TurnRight:
				case CommandType.Clean:
					return true;
				case CommandType.Advance:
				case CommandType.Back:
					var cell = GetCurrentRobotCell();
					return true;
					break;
				default:
					error = $"CommandType '{command.CommandType}' is invalid";
					return false;
			}
		}

		private Cell[,] ConvertToRectangularArray(string[][] jaggedArray)
		{
			int rows = jaggedArray.Length;
			int cols = jaggedArray.Max(row => row.Length);
			Cell[,] rectangularArray = new Cell[rows, cols];

			for (int y = 0; y < rows; y++)
			{
				for (int x = 0; x < jaggedArray[y].Length; x++)
				{
					rectangularArray[y, x] = new Cell(y, x, jaggedArray[y][x].ToCellType());
				}
			}

			return rectangularArray;
		}

		private void Clean(Cell cell)
		{
			_map.Cells[cell.X, cell.Y].State = CellState.Cleaned;
		}

		private void Visit(Cell cell)
		{
			_map.Cells[cell.X, cell.Y].State = CellState.Visited;
		}

		private bool IsNextCellAwailable(Cell currentCell, Command command)
		{
			throw new NotImplementedException();
		}

		private Cell GetCurrentRobotCell()
		{
			var robot = _robotController.GetCurrentStatus();
			return _map.Cells[robot.X, robot.Y];
		}
	}
}

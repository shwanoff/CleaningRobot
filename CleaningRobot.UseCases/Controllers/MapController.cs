using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.Entities.Extensions;
using CleaningRobot.UseCases.Dto;
using CleaningRobot.UseCases.Helpers;
using CleaningRobot.UseCases.Interfaces.Controllers;
using CleaningRobot.UseCases.Interfaces.Operators;

namespace CleaningRobot.UseCases.Controllers
{
	internal class MapController(IRobotOperator robotOperation) : IMapController, IMapOperator
	{
		private bool IsMapCreated => _map != null;

		private readonly IRobotOperator _robotOperation = robotOperation;

		private Map _map;

		public void Create(string[][] map)
		{
			_map = new Map(ConvertToRectangularArray(map));
		}

		public IEnumerable<CellStatusDto> GetAllCellStatuses()
		{
			if (!IsMapCreated)
			{
				throw new InvalidOperationException("The map has not been created yet");
			}

			for (int y = 0; y < _map.Height; y++)
			{
				for (int x = 0; x < _map.Width; x++)
				{
					yield return new CellStatusDto
					{
						X = x,
						Y = y,
						Type = _map.Cells[y, x].Type,
						State = _map.Cells[y, x].State
					};
				}
			}
		}

		public Cell GetCellStatus(int x, int y)
		{
			if (!IsMapCreated)
			{
				throw new InvalidOperationException("The map has not been created yet");
			}

			return _map.Cells[x, y];
		}

		public void Update(Command command)
		{
			if (command == null)
			{
				throw new ArgumentNullException(nameof(command), "Command cannot be null");
			}

			if (!IsMapCreated)
			{
				throw new InvalidOperationException("The map has not been created yet");
			}

			if (ValidateCommand(command, out string? error))
			{
				switch (command.CommandType)
				{
					case CommandType.Clean:
						Clean();
						break;
					case CommandType.Advance:
					case CommandType.Back:
						Visit();
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

			if (!IsMapCreated)
			{
				error = "The map has not been created yet";
				return false;
			}

			if (command == null)
			{
				error = "Command cannot be null";
				return false;
			}

			switch (command.CommandType)
			{
				case CommandType.TurnLeft:
				case CommandType.TurnRight:
				case CommandType.Clean:
					return true;
				case CommandType.Advance:
				case CommandType.Back:
					return IsNextCellAwailable(command, out error);
				default:
					error = $"CommandType '{command.CommandType}' is invalid";
					return false;
			}
		}

		public int GetMapWidth()
		{
			if (!IsMapCreated)
			{
				throw new InvalidOperationException("The map has not been created yet");
			}

			return _map.Width;
		}

		public int GetMapHeight()
		{
			if (!IsMapCreated)
			{
				throw new InvalidOperationException("The map has not been created yet");
			}

			return _map.Height;
		}

		private Cell GetCurrentRobotCell()
		{
			var robotPosition = _robotOperation.GetRobotPosition();
			return _map.Cells[robotPosition.X, robotPosition.Y];
		}

		private void Clean()
		{
			var cell = GetCurrentRobotCell();
			_map.Cells[cell.Position.X, cell.Position.Y].State = CellState.Cleaned;
		}

		private void Visit()
		{
			var cell = GetCurrentRobotCell();
			_map.Cells[cell.Position.X, cell.Position.Y].State = CellState.Visited;
		}

		private bool IsNextCellAwailable(Command command, out string? error)
		{
			error = null;
			var robotPosition = _robotOperation.GetRobotPosition();
			var nextPositon = PositionHelper.GetNextPosition(robotPosition, command.CommandType);

			if (nextPositon.X < 0 || nextPositon.X >= _map.Width || nextPositon.Y < 0 || nextPositon.Y >= _map.Height)
			{
				error = "The robot cannot move outside the map";
				return false;
			}

			var nextCell = _map.Cells[nextPositon.X, nextPositon.Y];

			if (nextCell.Type == CellType.Wall)
			{
				error = "The robot cannot move into a wall";
				return false;
			}

			if (nextCell.Type == CellType.Column)
			{
				error = "The robot cannot move into a column";
				return false;
			}

			return true;
		}

		private static Cell[,] ConvertToRectangularArray(string[][] jaggedArray)
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
	}
}

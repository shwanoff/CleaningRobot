using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto;
using CleaningRobot.UseCases.Interfaces.Controllers;
using CleaningRobot.UseCases.Interfaces.Operations;

namespace CleaningRobot.UseCases.Controllers
{
	public class MapController : IMapController, IMapOperation
	{
		public Map Create(string[][] map)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<CellStatusDto> GetAllCellStatuses()
		{
			throw new NotImplementedException();
		}

		public void Update(Command command)
		{
			throw new NotImplementedException();
		}

		public bool ValidateCommand(Command command, out string error)
		{
			throw new NotImplementedException();
		}
	}
}

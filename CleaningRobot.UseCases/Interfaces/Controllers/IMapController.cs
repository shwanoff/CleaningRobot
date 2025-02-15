using CleaningRobot.UseCases.Dto;

namespace CleaningRobot.UseCases.Interfaces.Controllers
{
	public interface IMapController
	{
		void Create(string[][] map);
		IEnumerable<CellStatusDto> GetAllCellStatuses();
	}
}

using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto;

namespace CleaningRobot.UseCases.Interfaces.Controllers
{
    public interface IMapController
    {
        Map Create(string[][] map);
        IEnumerable<CellStatusDto> GetAllCellStatuses();

	}
}

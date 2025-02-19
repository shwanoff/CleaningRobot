using CleaningRobot.UseCases.Dto.Input;
using CleaningRobot.UseCases.Dto.Output;

namespace CleaningRobot.UseCases.Interfaces.Controllers
{
	/// <summary>
	/// Provides methods to control the creation and retrieval of map data transfer objects (DTOs).
	/// </summary>
	public interface IMapController : IController<MapDataDto, MapStatusDto>
	{

	}
}
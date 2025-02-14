using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Dto;

namespace CleaningRobot.UseCases.Interfaces.Controllers
{
	/// <summary>
	/// Map controller interface. Responsible for external map operations
	/// </summary>
	public interface IMapController
	{
		/// <summary>
		/// Create a new map.
		/// </summary>
		/// <param name="map">A 2D array representing the map layout.</param>
		void Create(string[][] map);

		/// <summary>
		/// Retrieve the status of all cells in the map.
		/// </summary>
		/// <returns>Returns an enumerable collection of CellStatusDto objects.</returns>
		IEnumerable<CellStatusDto> GetAllCellStatuses();
	}
}

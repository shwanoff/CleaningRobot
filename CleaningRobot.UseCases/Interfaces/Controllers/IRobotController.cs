using CleaningRobot.UseCases.Dto;

namespace CleaningRobot.UseCases.Interfaces.Controllers
{
	/// <summary>
	/// Robot controller interface. Responsible for external robot operations
	/// </summary>
	public interface IRobotController
	{
		/// <summary>
		/// Creates a new robot with the specified parameters.
		/// </summary>
		/// <param name="x">The x-coordinate of the robot's initial position.</param>
		/// <param name="y">The y-coordinate of the robot's initial position.</param>
		/// <param name="facing">The initial direction the robot is facing.</param>
		/// <param name="battery">The initial battery level of the robot.</param>
		void Create(int x, int y, string facing, int battery);

		/// <summary>
		/// Gets the current status of the robot.
		/// </summary>
		/// <returns>An object containing the current status of the robot.</returns>
		RobotStatusDto GetCurrentStatus();
	}
}

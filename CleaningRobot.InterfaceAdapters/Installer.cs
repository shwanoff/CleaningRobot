using CleaningRobot.InterfaceAdapters.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CleaningRobot.InterfaceAdapters
{
	public static class Installer
	{
		public static void AddInterfaceAdapters(this IServiceCollection services)
		{
			services.AddTransient<IRobotOrchestrator, RobotOrchestrator>();
		}
	}
}

using CleaningRobot.InterfaceAdapters;
using CleaningRobot.InterfaceAdapters.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleaningRobot
{
	public static class Build
	{
		public static ServiceProvider ConfigureServices()
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();

			var services = new ServiceCollection()
				.AddSingleton<IConfiguration>(configuration)
				.AddInterfaceAdapters(configuration);

			var serviceProvider = services.BuildServiceProvider();

			return serviceProvider;
		}
	}
}

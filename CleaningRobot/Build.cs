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
			var services = new ServiceCollection()
				.LoadConfiguration()
				.AddInterfaceAdapters();

			var serviceProvider = services.BuildServiceProvider();

			return serviceProvider;
		}

		private static IServiceCollection LoadConfiguration(this IServiceCollection services)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();

			services.AddSingleton<IConfiguration>(configuration);

			return services;
		}
	}
}

using CleaningRobot.InterfaceAdapters;
using CleaningRobot.UseCases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CleaningRobot
{
	public static class Startup
	{
		public static ServiceProvider ConfigureServices()
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();

			var services = new ServiceCollection()
				.AddSingleton<IConfiguration>(configuration)
				.AddInterfaceAdapters(configuration)
				.AddUseCases()
				.AddMediatRServices(
					"CleaningRobot.Entities",
					"CleaningRobot.UseCases",
					"CleaningRobot.InterfaceAdapters");

			var serviceProvider = services.BuildServiceProvider();

			return serviceProvider;
		}

		public static IServiceCollection AddMediatRServices(this IServiceCollection services, params string[] assemblyNames)
		{
			services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

			services.AddMediatR(cfg =>
			{
				foreach (var assemblyName in assemblyNames)
					cfg.RegisterServicesFromAssembly(Assembly.Load(assemblyName));
			});

			return services;
		}
	}
}

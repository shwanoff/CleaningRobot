using CleaningRobot.Entities.Interfaces;
using CleaningRobot.InterfaceAdapters;
using CleaningRobot.InterfaceAdapters.Adapters;
using CleaningRobot.InterfaceAdapters.Dto;
using CleaningRobot.InterfaceAdapters.Interfaces;
using CleaningRobot.UseCases;
using CleaningRobot.UseCases.Controllers;
using CleaningRobot.UseCases.Interfaces.Controllers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace CleaningRobot.Tests.IntegrationTests
{
	[TestFixture]
	public class RobotOrchestratorIntegrationTests
	{
		private IRobotController _robotController;
		private IMapController _mapController;
		private ICommandController _commandController;
		private IFileAdapter _fileAdapter;
		private IJsonAdapter _jsonAdapter;
		private ILogAdapter _logAdapter;
		private EnergyConsumptionConfigurationDto _energyConsumptionConfig;
		private BackOffStrategiesConfigurationDto _backOffStrategiesConfig;
		private TxtLogConfigurationDto _txtLogConfigurationDto;
		private RobotOrchestrator _robotOrchestrator;
		private IMediator _mediator;

		[SetUp]
		public void SetUp()
		{

			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("TestData\\Inputs\\testsettings.json", optional: false, reloadOnChange: true)
				.Build();

			var services = new ServiceCollection()
				.AddSingleton<IConfiguration>(configuration)
				.AddInterfaceAdapters(configuration)
				.AddUseCases()
				.AddMediatRServices(
					"CleaningRobot.Entities",
					"CleaningRobot.UseCases",
					"CleaningRobot.InterfaceAdapters")
				.BuildServiceProvider();

			_mediator = services.GetRequiredService<IMediator>();
			_txtLogConfigurationDto = services.GetRequiredService<TxtLogConfigurationDto>();
			_energyConsumptionConfig = services.GetRequiredService<EnergyConsumptionConfigurationDto>();
			_backOffStrategiesConfig = services.GetRequiredService<BackOffStrategiesConfigurationDto>();

			_robotController = new RobotController(_mediator);
			_mapController = new MapController(_mediator);
			_commandController = new CommandContoller(_mediator);
			_fileAdapter = new FileAdapter();
			_jsonAdapter = new JsonAdapter();
			_logAdapter = new TxtLogAdapter(_fileAdapter, _txtLogConfigurationDto);

			_robotOrchestrator = new RobotOrchestrator(
				_robotController,
				_mapController,
				_commandController,
				_fileAdapter,
				_jsonAdapter,
				_logAdapter,
				_energyConsumptionConfig,
				_backOffStrategiesConfig);
		}

		[TestCase("TestData\\Inputs\\test1.json", "TestData\\Actual\\test1_actual_result.json", "TestData\\Results\\test1_result.json")]
		[TestCase("TestData\\Inputs\\test2.json", "TestData\\Actual\\test2_actual_result.json", "TestData\\Results\\test2_result.json")]
		public async Task ExecuteAsync_WithTestInput_ReturnsExpectedResult(string inputFilePath, string outputFilePath, string expectedOutputFilePath)
		{
			// Arrange
			var path = Directory.GetCurrentDirectory();
			var inputPath = Path.Combine(path, inputFilePath);
			var outputPath = Path.Combine(path, outputFilePath);
			var expectedPath = Path.Combine(path, expectedOutputFilePath);

			var expectedOutputJson = await File.ReadAllTextAsync(expectedPath);
			var expectedOutput = await _jsonAdapter.DeserializeAsync<OutputDataDto>(expectedOutputJson);

			// Act
			var result = await _robotOrchestrator.ExecuteAsync(inputPath, outputPath);

			// Asserts
			var actualOutputJson = await File.ReadAllTextAsync(outputPath);
			var actualOutput = await _jsonAdapter.DeserializeAsync<OutputDataDto>(actualOutputJson);
			
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Success, Is.True);
			Assert.That(result.Error, Is.Null);
			Assert.That(result.Result, Is.Not.Null);
			Assert.That(result.Result, Is.EqualTo(actualOutputJson));

			Assert.That(actualOutput.Visited.Count, Is.EqualTo(expectedOutput.Visited.Count));
			for (int i = 0; i < actualOutput.Visited.Count; i++)
			{
				Assert.That(actualOutput.Visited[i].X, Is.EqualTo(expectedOutput.Visited[i].X));
				Assert.That(actualOutput.Visited[i].Y, Is.EqualTo(expectedOutput.Visited[i].Y));
			}

			Assert.That(actualOutput.Cleaned.Count, Is.EqualTo(expectedOutput.Cleaned.Count));
			for (int i = 0; i < actualOutput.Cleaned.Count; i++)
			{
				Assert.That(actualOutput.Cleaned[i].X, Is.EqualTo(expectedOutput.Cleaned[i].X));
				Assert.That(actualOutput.Cleaned[i].Y, Is.EqualTo(expectedOutput.Cleaned[i].Y));
			}

			Assert.That(actualOutput.Final.X, Is.EqualTo(expectedOutput.Final.X));
			Assert.That(actualOutput.Final.Y, Is.EqualTo(expectedOutput.Final.Y));
			Assert.That(actualOutput.Final.Facing, Is.EqualTo(expectedOutput.Final.Facing));

			Assert.That(actualOutput.Battery, Is.EqualTo(expectedOutput.Battery));
		}
	}
}
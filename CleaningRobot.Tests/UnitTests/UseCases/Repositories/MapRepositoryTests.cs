using CleaningRobot.Entities.Entities;
using CleaningRobot.UseCases.Repositories;

namespace CleaningRobot.Tests.UnitTests.UseCases.Repositories
{
	[TestFixture]
	public class MapRepositoryTests
	{
		private MapRepository _repository;

		[SetUp]
		public void SetUp()
		{
			_repository = new MapRepository();
		}

		[Test]
		public async Task AddAsync_AddsNewMap()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var cells = new Cell[10, 10];
			var map = new Map(cells);

			// Act
			var result = await _repository.AddAsync(map, executionId);

			// Assert
			Assert.That(result, Is.EqualTo(map));
		}

		[Test]
		public async Task DeleteAsync_RemovesMap()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var cells = new Cell[10, 10];
			var map = new Map(cells);
			await _repository.AddAsync(map, executionId);

			// Act
			await _repository.DeleteAsync(executionId);

			// Assert
			Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.GetByIdAsync(executionId));
		}

		[Test]
		public async Task GetAllAsync_ReturnsAllMaps()
		{
			// Arrange
			var executionId1 = Guid.NewGuid();
			var executionId2 = Guid.NewGuid();
			var cells1 = new Cell[10, 10];
			var cells2 = new Cell[10, 10];
			var map1 = new Map(cells1);
			var map2 = new Map(cells2);
			await _repository.AddAsync(map1, executionId1);
			await _repository.AddAsync(map2, executionId2);

			// Act
			var result = await _repository.GetAllAsync();

			// Assert
			Assert.That(result, Has.Exactly(2).Items);
		}

		[Test]
		public async Task GetByIdAsync_ReturnsMapById()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var cells = new Cell[10, 10];
			var map = new Map(cells);
			await _repository.AddAsync(map, executionId);

			// Act
			var result = await _repository.GetByIdAsync(executionId);

			// Assert
			Assert.That(result, Is.EqualTo(map));
		}

		[Test]
		public async Task UpdateAsync_UpdatesMap()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var cells = new Cell[10, 10];
			var map = new Map(cells);
			await _repository.AddAsync(map, executionId);
			var newCells = new Cell[20, 20];
			var newMap = new Map(newCells);

			// Act
			var result = await _repository.UpdateAsync(newMap, executionId);

			// Assert
			Assert.That(result.Cells, Is.EqualTo(newMap.Cells));
		}
	}
}
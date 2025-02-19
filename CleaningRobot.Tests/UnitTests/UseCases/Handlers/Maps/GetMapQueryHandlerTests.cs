using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.UseCases.Handlers.Maps;
using CleaningRobot.UseCases.Interfaces.Repositories;
using Moq;

namespace CleaningRobot.Tests.UnitTests.UseCases.Handlers.Maps
{
	[TestFixture]
	public class GetMapQueryHandlerTests
	{
		private Mock<IRepository<Map>> _mapRepositoryMock;
		private GetMapQueryHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_mapRepositoryMock = new Mock<IRepository<Map>>();
			_handler = new GetMapQueryHandler(_mapRepositoryMock.Object);
		}

		[Test]
		public async Task Handle_ValidRequest_ReturnsCorrectStatus()
		{
			// Arrange
			var executionId = Guid.NewGuid();
			var cells = new Cell[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					cells[i, j] = new Cell(i, j, CellType.CleanableSpace)
					{
						State = CellState.NotVisited
					};
				}
			}
			var map = new Map(cells);

			_mapRepositoryMock.Setup(repo => repo.GetByIdAsync(executionId)).ReturnsAsync(map);

			var request = new GetMapQuery
			{
				ExecutionId = executionId
			};

			// Act
			var result = await _handler.Handle(request, CancellationToken.None);

			// Assert
			Assert.That(result.ExecutionId, Is.EqualTo(executionId));
			Assert.That(result.IsCorrect, Is.True);
			Assert.That(result.Width, Is.EqualTo(3));
			Assert.That(result.Height, Is.EqualTo(3));
			Assert.That(result.Cells.Count, Is.EqualTo(9));
			Assert.That(result.Cells.All(c => c.Type == CellType.CleanableSpace), Is.True);
		}
	}
}
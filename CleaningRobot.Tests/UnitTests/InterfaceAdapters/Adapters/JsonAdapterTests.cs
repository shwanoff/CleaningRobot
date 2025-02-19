using CleaningRobot.Entities.Entities;
using CleaningRobot.Entities.Enums;
using CleaningRobot.InterfaceAdapters.Adapters;

namespace CleaningRobot.Tests.UnitTests.InterfaceAdapters.Adapters
{
	[TestFixture]
	class JsonAdapterTests
    {
		private JsonAdapter _jsonAdapter;

		[SetUp]
		public void SetUp()
		{
			_jsonAdapter = new JsonAdapter();
		}

		[Test]
		public async Task SerializeAsync_RobotPosition_ReturnsCorrectJson()
		{
			var robotPosition = new RobotPosition(1, 2, Facing.North);
			var json = await _jsonAdapter.SerializeAsync(robotPosition);
			var expectedJson = "{\n  \"Facing\": \"N\",\n  \"X\": 1,\n  \"Y\": 2\n}";
			Assert.That(Normalize(json), Is.EqualTo(expectedJson));
		}

		[Test]
		public async Task DeserializeAsync_RobotPosition_ReturnsCorrectObject()
		{
			var json = "{\"X\":1,\"Y\":2,\"Facing\":\"N\"}";
			var robotPosition = await _jsonAdapter.DeserializeAsync<RobotPosition>(json);
			Assert.That(robotPosition.X, Is.EqualTo(1));
			Assert.That(robotPosition.Y, Is.EqualTo(2));
			Assert.That(robotPosition.Facing, Is.EqualTo(Facing.North));
		}

		private static string Normalize(string json)
		{
			return json.Replace("\r\n", "\n").Replace("\r", "\n");
		}
	}
}

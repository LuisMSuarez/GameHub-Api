namespace GameHubApiTests
{
    using GameHubApi.Contracts;
    using GameHubApi.Controllers;
    using GameHubApi.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class GamesControllerTests
    {
        private readonly Mock<IGamesService> mockGamesService;
        private readonly Mock<ILogger<GamesController>> mockLogger;
        private readonly GamesController controller;

        public GamesControllerTests()
        {
            mockGamesService = new Mock<IGamesService>();
            mockLogger = new Mock<ILogger<GamesController>>();
            controller = new GamesController(mockLogger.Object, mockGamesService.Object, Mock.Of<IConfiguration>());
        }

        [Fact]
        public async Task GetGamesAsync_ReturnsCollectionResult_WhenServiceReturnsData()
        {
            // Arrange
            var mockResult = new CollectionResult<Game>
            {
                count = 2,
                results = new List<Game>
                {
                    new Game { Id = 1, Name = "Game 1", Rating = 4.5 },
                    new Game { Id = 2, Name = "Game 2", Rating = 4.0 }
                }
            };

            mockGamesService
                .Setup(service => service.GetGamesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await controller.GetGamesAsync(null, null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.count);
            Assert.Equal("Game 1", result.results[0].Name);
            Assert.Equal("Game 2", result.results[1].Name);
        }

        [Fact]
        public async Task GetGamesAsync_ThrowsException_WhenServiceThrowsException()
        {
            // Arrange
            mockGamesService
                .Setup(service => service.GetGamesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => controller.GetGamesAsync(null, null, null, null));
        }
    }
}
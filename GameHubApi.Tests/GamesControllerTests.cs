namespace GameHubApiTests
{
    using Castle.Core.Configuration;
    using GameHubApi.Contracts;
    using GameHubApi.Controllers;
    using GameHubApi.Services;
    using Microsoft.AspNetCore.Mvc;
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
            controller = new GamesController(mockLogger.Object, mockGamesService.Object);
        }

        [Fact]
        public async Task GetGamesAsync_ReturnsCollectionResult_WhenServiceReturnsData()
        {
            // Arrange
            var mockResult = new CollectionResult<Game>
            {
                Count = 2,
                Results = new List<Game>
                {
                    new Game { Id = 1, Name = "Game 1", Slug="game1", Rating = 4.5 },
                    new Game { Id = 2, Name = "Game 2", Slug="game2", Rating = 4.0 }
                }
            };

            mockGamesService
                .Setup(service => service.GetGamesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(mockResult);

            // Act
            var actionResult = await controller.GetGamesAsync(null, null, null, null, 1, 20);
            var okResult = actionResult as OkObjectResult;
            Assert.NotNull(okResult);
            var result = okResult.Value as CollectionResult<Game>;

            // Assert
            Assert.Equal(2, result!.Count);
            Assert.Equal("Game 1", result.Results[0].Name);
            Assert.Equal("Game 2", result.Results[1].Name);
        }

        [Fact]
        public async Task GetGamesAsync_ThrowsException_WhenServiceThrowsException()
        {
            // Arrange
            mockGamesService
                .Setup(service => service.GetGamesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => controller.GetGamesAsync(null, null, null, null, 1, 20));
        }
    }
}
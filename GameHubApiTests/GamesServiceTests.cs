using GameHubApi.Contracts;
using GameHubApi.Providers;
using GameHubApi.Services;
using Moq;
using Xunit;

namespace GameHubApiTests
{
    public class GamesServiceTests
    {
        [Fact]
        public async Task GetGamesAsync_CallsRawgApi_WithCorrectParameters()
        {
            // Arrange
            var mockRawgApi = new Mock<IRawgApi>();
            var expectedResult = new CollectionResult<Game> { count = 10 };
            mockRawgApi
                .Setup(api => api.GetGamesAsync("action", "pc", "name", "search", 1, 20))
                .ReturnsAsync(expectedResult);

            var gamesService = new GamesService(mockRawgApi.Object);

            // Act
            var result = await gamesService.GetGamesAsync("action", "pc", "name", "search");

            // Assert
            Assert.Equal(expectedResult, result);
            mockRawgApi.Verify(api => api.GetGamesAsync("action", "pc", "name", "search", 1, 20), Times.Once);
        }

        [Fact]
        public async Task GetGamesAsync_ThrowsException_WhenRawgApiFails()
        {
            // Arrange
            var mockRawgApi = new Mock<IRawgApi>();
            mockRawgApi
                .Setup(api => api.GetGamesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new InvalidOperationException("API error"));

            var gamesService = new GamesService(mockRawgApi.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                gamesService.GetGamesAsync("action", "pc", "name", "search"));
        }
    }
}

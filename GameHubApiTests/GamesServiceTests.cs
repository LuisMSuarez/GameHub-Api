﻿namespace GameHubApiTests
{
    using GameHubApi.Contracts;
    using GameHubApi.Providers;
    using GameHubApi.Services;
    using Moq;

    public class GamesServiceTests
    {
        [Fact]
        public async Task GetGamesAsync_CallsRawgApi_WithCorrectParameters()
        {
            // Arrange
            var mockRawgApi = new Mock<IRawgApi>();
            var expectedResult = new CollectionResult<Game> { Count = 10 };
            mockRawgApi
                .Setup(api => api.GetGamesAsync("action", "pc", "name", "search", 1, 20))
                .ReturnsAsync(expectedResult);

            var gamesService = new GamesService(mockRawgApi.Object, Mock.Of<IGameFilter>());

            // Act
            var result = await gamesService.GetGamesAsync("action", "pc", "name", "search", 1, 20);

            // Assert
            Assert.Equal(expectedResult.Count, result.Count);
            mockRawgApi.Verify(api => api.GetGamesAsync("action", "pc", "name", "search", 1, 20), Times.Once);
        }

        [Fact]
        public async Task GetGamesAsync_GameFilter_FiltersContent()
        {
            // Arrange
            var mockRawgApi = new Mock<IRawgApi>();
            var apiGamesResult = new CollectionResult<Game>
            {
                Count = 2,
                Results = new List<Game> {
                        new Game { Name = "Game 1" , Slug="game1"},
                        new Game { Name = "Game 2" , Slug="game2"}
                    }
            };
            mockRawgApi
                .Setup(api => api.GetGamesAsync("action", "pc", "name", "search", 1, 20))
                .ReturnsAsync(apiGamesResult);

            var mockGameFilter = new Mock<IGameFilter>();
            mockGameFilter
                .Setup(filter => filter.Filter(It.Is<Game>(g => g.Name.Equals("Game 1"))))
                .Returns(FilterResult.Passed);
            mockGameFilter
                .Setup(filter => filter.Filter(It.Is<Game>(g => g.Name.Equals("Game 2"))))
                .Returns(FilterResult.Blocked);
            var gamesService = new GamesService(mockRawgApi.Object, mockGameFilter.Object);

            // Act
            var result = await gamesService.GetGamesAsync("action", "pc", "name", "search", 1, 20);

            // Assert
            Assert.Single(result.Results);
            Assert.DoesNotContain(result.Results, g => g.Name == "Game 2");
            mockRawgApi.Verify(api => api.GetGamesAsync("action", "pc", "name", "search", 1, 20), Times.Once);
        }

        [Fact]
        public async Task GetGamesAsync_ThrowsException_WhenRawgApiFails()
        {
            // Arrange
            var mockRawgApi = new Mock<IRawgApi>();
            var mockGameFilter = new Mock<IGameFilter>();
            mockRawgApi
                .Setup(api => api.GetGamesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new InvalidOperationException("API error"));

            var gamesService = new GamesService(mockRawgApi.Object, mockGameFilter.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                gamesService.GetGamesAsync("action", "pc", "name", "search", 1, 20));
        }
    }
}

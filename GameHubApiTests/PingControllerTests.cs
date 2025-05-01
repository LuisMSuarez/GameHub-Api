namespace GameHubApiTests
{
    using GameHubApi.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class PingControllerTests
    {
        private readonly Mock<ILogger<PingController>> mockLogger;
        private readonly PingController controller;

        public PingControllerTests()
        {
            mockLogger = new Mock<ILogger<PingController>>();
            controller = new PingController(mockLogger.Object);
        }

        [Fact]
        public void Ping_ReturnsOkResult_WithExpectedMessage()
        {
            // Act
            var result = controller.Ping();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            Assert.True(okResult.Value.ToString().Contains("Request was successful!"));
        }
    }
}
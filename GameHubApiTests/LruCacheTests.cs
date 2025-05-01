using GameHubApi.Providers;

namespace GameHubApiTests
{
    public class LruCacheTests
    {
        [Fact]
        public void Get_ReturnsValue_WhenKeyExists()
        {
            // Arrange  
            var cache = new LruCache<string, string>(size: 2);
            cache.Set("key1", "value1", null);

            // Act  
            var result = cache.Get("key1");

            // Assert  
            Assert.Equal("value1", result);
        }

        [Fact]
        public void Get_ReturnsNull_WhenKeyDoesNotExist()
        {
            // Arrange  
            var cache = new LruCache<string, string>(size: 2);

            // Act  
            var result = cache.Get("key1");

            // Assert  
            Assert.Null(result);
        }
    }
}

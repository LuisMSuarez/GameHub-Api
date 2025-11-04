namespace GameHubApi.Contracts
{
    using System.Text.Json.Serialization;
    public class GameRecommendationsRequest
    {
        [JsonPropertyName("likedGames")]
        public List<Game> LikedGames { get; set; } = new List<Game>();

        [JsonPropertyName("dislikedGames")]
        public List<Game> DislikedGames { get; set; } = new List<Game>();
    }
}

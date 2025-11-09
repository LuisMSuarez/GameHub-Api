namespace GameHubApi.Contracts
{
    // Enables JSON property name mapping for serialization/deserialization
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a request payload for generating personalized game recommendations.
    /// Contains lists of games the user has liked and disliked to guide recommendation logic.
    /// </summary>
    public class GameRecommendationsRequest
    {
        /// <summary>
        /// List of games the user has positively rated or enjoyed.
        /// Used as input to identify similar or related titles.
        /// </summary>
        [JsonPropertyName("likedGames")]
        public List<Game> LikedGames { get; set; } = new List<Game>();

        /// <summary>
        /// List of games the user has negatively rated or did not enjoy.
        /// Helps filter out unwanted genres, mechanics, or themes.
        /// </summary>
        [JsonPropertyName("dislikedGames")]
        public List<Game> DislikedGames { get; set; } = new List<Game>();
    }
}

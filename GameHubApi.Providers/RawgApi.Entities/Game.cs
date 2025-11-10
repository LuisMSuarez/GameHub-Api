namespace GameHubApi.Providers.RawgApiEntities
{
    using GameHubApi.Contracts;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a game entity as returned by the RAWG API.
    /// This internal model is used for deserialization and transformation into the public-facing contract.
    /// </summary>
    internal class Game
    {
        /// <summary>
        /// Indicates whether the game entry is a redirect placeholder.
        /// If true, the game should not be converted or used directly.
        /// </summary>
        [JsonPropertyName("redirect")]
        public bool? Redirect { get; set; }

        /// <summary>
        /// Unique identifier for the game.
        /// </summary>
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Display name of the game.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// URL-friendly identifier for the game.
        /// </summary>
        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        /// <summary>
        /// Optional background image URL for the game.
        /// </summary>
        [JsonPropertyName("background_image")]
        public string? BackgroundImage { get; set; }

        /// <summary>
        /// Average user rating for the game.
        /// </summary>
        [JsonPropertyName("rating")]
        public double? Rating { get; set; }

        /// <summary>
        /// Metacritic score for the game, if available.
        /// </summary>
        [JsonPropertyName("metacritic")]
        public int? Metacritic { get; set; }

        /// <summary>
        /// Highest possible rating value (e.g., 5 or 100).
        /// </summary>
        [JsonPropertyName("rating_top")]
        public int? RatingTop { get; set; }

        /// <summary>
        /// List of parent platforms the game supports (e.g., PlayStation, Xbox).
        /// </summary>
        [JsonPropertyName("parent_platforms")]
        public IList<ParentPlatform> ParentPlatforms { get; set; } = new List<ParentPlatform>();

        /// <summary>
        /// List of descriptive tags associated with the game.
        /// </summary>
        [JsonPropertyName("tags")]
        public IList<Tag> Tags { get; set; } = new List<Tag>();

        /// <summary>
        /// Raw textual description of the game.
        /// </summary>
        [JsonPropertyName("description_raw")]
        public string? Description { get; set; }

        /// <summary>
        /// List of genres the game belongs to (e.g., Action, RPG).
        /// </summary>
        [JsonPropertyName("genres")]
        public IList<Genre> Genres { get; set; } = new List<Genre>();

        /// <summary>
        /// List of publishers responsible for releasing the game.
        /// </summary>
        [JsonPropertyName("publishers")]
        public IList<Publisher> Publishers { get; set; } = new List<Publisher>();

        /// <summary>
        /// Converts the internal RAWG API game model into the public-facing Game contract.
        /// Throws exceptions if required fields are missing or if the game is a redirect.
        /// </summary>
        /// <returns>A fully populated <see cref="GameHubApi.Contracts.Game"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the game is a redirect or missing required fields.</exception>
        public GameHubApi.Contracts.Game ToContract()
        {
            if (this.Redirect.HasValue && this.Redirect.Value)
            {
                throw new InvalidOperationException("Cannot convert a redirected game to contract.");
            }

            if (!this.Id.HasValue ||
                string.IsNullOrWhiteSpace(this.Name) ||
                string.IsNullOrWhiteSpace(this.Slug))
            {
                throw new InvalidOperationException("Missing required game properties to convert to contract.");
            }

            return new GameHubApi.Contracts.Game
            {
                Id = this.Id.Value,
                Name = this.Name!,
                Slug = this.Slug!,
                BackgroundImage = this.BackgroundImage,
                Rating = this.Rating ?? 0,
                Metacritic = this.Metacritic,
                RatingTop = this.RatingTop ?? 0,
                ParentPlatforms = this.ParentPlatforms,
                Tags = this.Tags,
                Description = this.Description ?? string.Empty,
                Genres = this.Genres,
                Publishers = this.Publishers
            };
        }
    }
}

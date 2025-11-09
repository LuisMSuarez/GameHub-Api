namespace GameHubApi.Contracts
{
    // Enables JSON serialization attributes for mapping C# properties to JSON keys
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a video game entity with metadata used in the GameHub API.
    /// Implements ICloneable to support shallow copying of game instances.
    /// </summary>
    public class Game : ICloneable
    {
        /// <summary>
        /// Unique identifier for the game.
        /// </summary>
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        /// <summary>
        /// Display name of the game.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// URL-friendly identifier (typically lowercase with hyphens).
        /// </summary>
        [JsonPropertyName("slug")]
        public required string Slug { get; set; }

        /// <summary>
        /// Optional URL to the game's background image.
        /// </summary>
        [JsonPropertyName("background_image")]
        public string? BackgroundImage { get; set; }

        /// <summary>
        /// Average user rating (e.g., out of 5).
        /// </summary>
        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        /// <summary>
        /// Optional Metacritic score (0–100).
        /// </summary>
        [JsonPropertyName("metacritic")]
        public int? Metacritic { get; set; }

        /// <summary>
        /// Highest possible rating value (e.g., 5 or 10).
        /// </summary>
        [JsonPropertyName("rating_top")]
        public int RatingTop { get; set; }

        /// <summary>
        /// List of platforms the game is available on (e.g., PC, Xbox).
        /// </summary>
        [JsonPropertyName("parent_platforms")]
        public IList<ParentPlatform> ParentPlatforms { get; set; } = new List<ParentPlatform>();

        /// <summary>
        /// List of descriptive tags associated with the game (e.g., "Multiplayer", "Indie").
        /// </summary>
        [JsonPropertyName("tags")]
        public IList<Tag> Tags { get; set; } = new List<Tag>();

        /// <summary>
        /// Optional raw text description of the game.
        /// </summary>
        [JsonPropertyName("description_raw")]
        public string? Description { get; set; }

        /// <summary>
        /// List of genres the game belongs to (e.g., "Action", "RPG").
        /// </summary>
        [JsonPropertyName("genres")]
        public IList<Genre> Genres { get; set; } = new List<Genre>();

        /// <summary>
        /// List of publishers responsible for releasing the game.
        /// </summary>
        [JsonPropertyName("publishers")]
        public IList<Publisher> Publishers { get; set; } = new List<Publisher>();

        /// <summary>
        /// Creates a shallow copy of the current Game instance.
        /// Note: Lists are not deep-copied; they will reference the same objects.
        /// </summary>
        public object Clone()
        {
            return new Game
            {
                Id = this.Id,
                Name = this.Name,
                Slug = this.Slug,
                BackgroundImage = this.BackgroundImage,
                Rating = this.Rating,
                Metacritic = this.Metacritic,
                RatingTop = this.RatingTop,
                ParentPlatforms = this.ParentPlatforms,
                Tags = this.Tags,
                Description = this.Description,
                Genres = this.Genres,
                Publishers = this.Publishers
            };
        }
    }
}

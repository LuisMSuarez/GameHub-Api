namespace GameHubApi.Contracts
{
    using System.Text.Json.Serialization;
    public class Game : ICloneable
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("slug")]
        public required string Slug { get; set; }

        [JsonPropertyName("background_image")]
        public string? BackgroundImage { get; set; }

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("metacritic")]
        public int? Metacritic { get; set; }

        [JsonPropertyName("rating_top")]
        public int RatingTop { get; set; }

        [JsonPropertyName("parent_platforms")]
        public IList<ParentPlatform> ParentPlatforms { get; set; } = new List<ParentPlatform>();

        [JsonPropertyName("tags")]
        public IList<Tag> Tags { get; set; } = new List<Tag>();

        [JsonPropertyName("description_raw")]
        public string? Description { get; set; }

        [JsonPropertyName("genres")]
        public IList<Genre> Genres { get; set; } = new List<Genre>();

        [JsonPropertyName("publishers")]
        public IList<Publisher> Publishers { get; set; } = new List<Publisher>();

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

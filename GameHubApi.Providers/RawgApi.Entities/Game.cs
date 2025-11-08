using GameHubApi.Contracts;
using System.Text.Json.Serialization;

namespace GameHubApi.Providers.RawgApiEntities
{
    internal class Game
    {
        [JsonPropertyName("redirect")]
        public bool? Redirect { get; set; }

        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [JsonPropertyName("background_image")]
        public string? BackgroundImage { get; set; }

        [JsonPropertyName("rating")]
        public double? Rating { get; set; }

        [JsonPropertyName("metacritic")]
        public int? Metacritic { get; set; }

        [JsonPropertyName("rating_top")]
        public int? RatingTop { get; set; }

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

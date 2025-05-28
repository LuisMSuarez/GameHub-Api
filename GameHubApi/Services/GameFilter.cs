using GameHubApi.Contracts;

namespace GameHubApi.Services
{
    public class GameFilter : IGameFilter
    {
        private const string BlockedTagsKey = "BlockedTags";
        private readonly string[] blockedTags;

        public GameFilter(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var blockedTagsValue = configuration[BlockedTagsKey];
            this.blockedTags = string.IsNullOrWhiteSpace(blockedTagsValue)
                ? Array.Empty<string>()
                : blockedTagsValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(tag => tag.Trim())
                                  .ToArray();

        }
        public FilterResult Filter(Game game)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            // Check if the game has any blocked tags
            if (game.Tags != null &&
                game.Tags.Any(tag => blockedTags.Contains(tag.Name, StringComparer.OrdinalIgnoreCase)))
            {
                return FilterResult.Blocked;
            }

            // Check if the game name contains any blocked tags
            if (blockedTags.Any(tag => game.Name.Contains(tag, StringComparison.OrdinalIgnoreCase)))
            {
                return FilterResult.Blocked;
            }

            // Check if the description contains any blocked tags
            if (!string.IsNullOrWhiteSpace(game.Description) &&
                blockedTags.Any(tag => game.Description.Contains(tag, StringComparison.OrdinalIgnoreCase)))
            {
                return FilterResult.Blocked;
            }

            return FilterResult.Passed;
        }
    }
}

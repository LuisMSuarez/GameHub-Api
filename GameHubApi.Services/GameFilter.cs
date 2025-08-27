namespace GameHubApi.Services
{
    using GameHubApi.Contracts;
    using Microsoft.Extensions.Configuration;

    public class GameFilter : IGameFilter
    {
        private const string BlockedTagsKey = "BlockedTags";
        private readonly string[] blockedTags;

        public GameFilter(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var blockedTagsValue = configuration[BlockedTagsKey];
            this.blockedTags = string.IsNullOrWhiteSpace(blockedTagsValue)
                ? []
                : blockedTagsValue.Split([','], StringSplitOptions.RemoveEmptyEntries)
                                  .Select(tag => tag.Trim())
                                  .ToArray();

        }
        public FilterResult Filter(Game game)
        {
            ArgumentNullException.ThrowIfNull(game);

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

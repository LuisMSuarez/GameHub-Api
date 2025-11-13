namespace GameHubApi.Services
{
    using GameHubApi.Contracts;
    using GameHubApi.Providers;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Provides rule-based filtering logic to determine whether a game is appropriate for platform inclusion.
    /// </summary>
    public class GameFilter : IGameFilter
    {
        private const string BlockedTagsKey = "BlockedTags";
        private readonly string[] blockedTags;
        private readonly IGameFilter aiGameFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameFilter"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration used to retrieve blocked tags.</param>
        /// <param name="gameFilterFactory">Factory function to resolve additional filter strategies (e.g., AI-based).</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> or the resolved AI filter is null.</exception>
        public GameFilter(
            IConfiguration configuration,
            Func<string, IGameFilter> gameFilterFactory)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var blockedTagsValue = configuration[BlockedTagsKey];
            this.blockedTags = string.IsNullOrWhiteSpace(blockedTagsValue)
                ? []
                : blockedTagsValue.Split([','], StringSplitOptions.RemoveEmptyEntries)
                                  .Select(tag => tag.Trim())
                                  .ToArray();

            this.aiGameFilter = gameFilterFactory("AI") ?? throw new ArgumentNullException(nameof(aiGameFilter));
        }

        /// <summary>
        /// Evaluates a game using static tag and keyword rules to determine if it should be blocked or passed.
        /// </summary>
        /// <param name="game">The game to evaluate.</param>
        /// <returns>
        /// A <see cref="FilterResult"/> indicating whether the game is <c>Blocked</c> or <c>Passed</c>.
        /// </returns>
        public Task<FilterResult> FilterAsync(Game game)
        {
            ArgumentNullException.ThrowIfNull(game);

            // Check if the game has any blocked tags
            if (game.Tags != null &&
                game.Tags.Any(tag => blockedTags.Contains(tag.Name, StringComparer.OrdinalIgnoreCase)))
            {
                return Task.FromResult(FilterResult.Blocked);
            }

            // Check if the game name contains any blocked tags
            if (blockedTags.Any(tag => game.Name.Contains(tag, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.FromResult(FilterResult.Blocked);
            }

            // Check if the description contains any blocked tags
            if (!string.IsNullOrWhiteSpace(game.Description) &&
                blockedTags.Any(tag => game.Description.Contains(tag, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.FromResult(FilterResult.Blocked);
            }

            // TODO: Use AI-based filtering for more complex content analysis
            // return this.aiGameFilter.FilterAsync(game);

            return Task.FromResult(FilterResult.Passed);
        }
    }
}

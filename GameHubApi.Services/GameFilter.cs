namespace GameHubApi.Services
{
    using GameHubApi.Contracts;
    using GameHubApi.Providers;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;

    public class GameFilter : IGameFilter
    {
        private const string BlockedTagsKey = "BlockedTags";
        private readonly string[] blockedTags;
        private readonly IGameFilter aiGameFilter;

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
        public async Task<FilterResult> FilterAsync(Game game)
        {
            // First stage filtering using local rules
            var localFilterResult = this.FilterLocal(game);
            if (localFilterResult != FilterResult.Passed)
            {
                return localFilterResult;
            }

            // Second stage filtering using AI
            return await this.aiGameFilter.FilterAsync(game);
        }

        public async Task<IEnumerable<FilterResult>> FilterAsync(IEnumerable<Game> games)
        {
            var localFilterResults = games.Select(g => new
            {
                Game = g,
                LocalFilter = this.FilterLocal(g)
            }).ToList();

            // For games that passed local filtering, apply AI filtering in a batch request.
            var aiFilterResult = (await this.aiGameFilter.FilterAsync(
                localFilterResults.Where(lf => lf.LocalFilter == FilterResult.Passed)
                           .Select(lf => lf.Game))).ToList();

            // Merge results, preserving order.
            var result = new List<FilterResult>();
            int aiIndex = 0;
            for (int i = 0; i< games.Count(); i++)
            {
                var localResult = localFilterResults[i];
                if (localResult.LocalFilter != FilterResult.Passed)
                {
                    result.Add(localResult.LocalFilter);
                }
                else
                {
                    result.Add(aiFilterResult[aiIndex++]);
                }
            }
            return result;
        }

        private FilterResult FilterLocal(Game game)
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

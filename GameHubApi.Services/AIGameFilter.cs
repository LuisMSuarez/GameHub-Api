using GameHubApi.Contracts;
using GameHubApi.Providers;
using GameHubApi.Providers.Contracts;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameHubApi.Services
{
    public class AIGameFilter : IGameFilter
    {
        private readonly ILargeLanguageModel largeLanguageModel;

        public AIGameFilter(ILargeLanguageModel largeLanguageModel)
        {
            this.largeLanguageModel = largeLanguageModel ?? throw new ArgumentNullException(nameof(largeLanguageModel));
        }

        public async Task<FilterResult> FilterAsync(Game game)
        {
            var result = await this.largeLanguageModel.GenerateResponseAsync(new GenerateResponseQuery
            {
                Instructions = "You are a content moderator for a gaming platform. Your task is to evaluate whether a game is suitable for general platform inclusion." +
                               "A game should be flagged as inappropriate only if it contains explicit or suggestive material that is primarily intended to provoke or appeal to adult-only interests, especially in ways that are not central to gameplay or narrative." +
                               "Respond with 'Blocked' if the game is not appropriate, otherwise respond with 'Passed'.",
                Query = "Game information in Json format:\n" + JsonSerializer.Serialize(game)
            });

            return result.Message.Contains("Blocked") ? FilterResult.Blocked : FilterResult.Passed;
        }

        public async Task<IEnumerable<FilterResult>> FilterAsync(IEnumerable<Game> games)
        {
            var result = await this.largeLanguageModel.GenerateResponseAsync(new GenerateResponseQuery
            {
                Instructions = "You are a content moderator for a gaming platform. Your task is to evaluate whether a game is suitable for general platform inclusion." +
                               "A game should be flagged as inappropriate only if it contains explicit or suggestive material that is primarily intended to provoke or appeal to adult-only interests, especially in ways that are not central to gameplay or narrative." +
                               "For each game, project either 'Blocked' if the game is not appropriate, otherwise project 'Passed'." +
                               "Respond with a Json array of strings, with no additional decoration.",
                Query = "Game list in Json format:\n" + JsonSerializer.Serialize(
                    // Project only necessary fields to reduce token usage
                    games.Select( g => new
                    {
                        g.Name,
                        g.Description
                    }))
            });

            var resultList = JsonSerializer.Deserialize<List<string>>(result.Message);
            if (resultList == null || resultList.Count != games.Count())
            {
                throw new InvalidOperationException("The response from the language model could not be parsed or does not match the number of games provided.");
            }

            return resultList.Select(r => r.Contains("Blocked") ? FilterResult.Blocked : FilterResult.Passed);
        }
    }
}

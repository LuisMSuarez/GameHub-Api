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
    }
}

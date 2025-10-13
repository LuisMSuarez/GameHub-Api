using GameHubApi.Contracts;
using GameHubApi.Providers;
using GameHubApi.Providers.Contracts;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameHubApi.Services
{
    public class AIGameFilter : IGameFilter
    {
        private const string Instructions =
                  "You are a content moderator for a gaming platform. Your task is to evaluate whether a game is suitable for general platform inclusion." +
                  "A game should be flagged as inappropriate only if it contains explicit or suggestive material that is primarily intended to provoke or appeal to adult-only interests, " +
                  "especially in ways that are not central to gameplay or narrative.";
        
            private readonly ILargeLanguageModel largeLanguageModel;

        public AIGameFilter(ILargeLanguageModel largeLanguageModel)
        {
            this.largeLanguageModel = largeLanguageModel ?? throw new ArgumentNullException(nameof(largeLanguageModel));
        }

        public async Task<FilterResult> FilterAsync(Game game)
        {
            var result = await this.largeLanguageModel.GenerateResponseAsync(new GenerateResponseQuery
            {
                Instructions = Instructions,
                Query = "Respond with 'Blocked' if the game is not appropriate, otherwise respond with 'Passed'." +
                        "Game information in Json format:\n" + JsonSerializer.Serialize(game)
            });

            return result.Message.Contains("Blocked") ? FilterResult.Blocked : FilterResult.Passed;
        }

        public async Task<IEnumerable<FilterResult>> FilterAsync(IEnumerable<Game> games)
        {
            var gamesSummary = games.Select(g => new
            {
                g.Id,
                g.Name,
                g.Description
            });

            var result = await this.largeLanguageModel.GenerateResponseAsync(new GenerateResponseQuery
            {
                Instructions = Instructions,
                Query = "For each game, project an object {Id, FilterResult} where Id is the game ID and FilterResult is either 'Blocked' if the game is not appropriate, otherwise 'Passed'." +
                        "Respond with an undecorated Json array of these objects." +
                        "Game list in Json format:\n" + JsonSerializer.Serialize(gamesSummary)
            });

            // Define the anonymous type template
            var template = new[] { new { Id = 1, FilterResult = "Passed" } } ;
            var deserializedResult = JsonSerializer.Deserialize(result.Message, template.GetType());
            var list = ((IEnumerable<object>)deserializedResult!).Cast<dynamic>().ToList();
            if (list == null || list.Count != gamesSummary.Count())
            {
                throw new InvalidOperationException("The response from the language model could not be parsed or does not match the number of games provided.");
            }

            return list.Select(r => r.FilterResult.Contains("Blocked") ? FilterResult.Blocked : FilterResult.Passed);
        }
    }
}

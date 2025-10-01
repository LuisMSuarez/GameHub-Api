using GameHubApi.Providers.Contracts;

namespace GameHubApi.Providers
{
    internal class OpenAILargeLanguageModel : ILargeLanguageModel
    {
        public Task<GenerateResponseResult> GenerateResponseAsync(GenerateResponseQuery query)
        {
            throw new NotImplementedException();
        }
    }
}

using GameHubApi.Providers.Contracts;

namespace GameHubApi.Providers
{
    public interface ILargeLanguageModel
    {
        Task<GenerateResponseResult> GenerateResponseAsync(GenerateResponseQuery query);
    }
}

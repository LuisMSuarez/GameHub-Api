using GameHubApi.Providers.Contracts;

namespace GameHubApi.Providers
{
    /// <summary>
    /// Defines a contract for interacting with a large language model to generate responses.
    /// </summary>
    public interface ILargeLanguageModel
    {
        /// <summary>
        /// Generates a response based on the provided query input.
        /// </summary>
        /// <param name="query">The input query containing prompt and context information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated response.</returns>
        Task<GenerateResponseResult> GenerateResponseAsync(GenerateResponseQuery query);
    }
}

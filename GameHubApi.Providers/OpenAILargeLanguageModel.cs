using GameHubApi.Providers.Contracts;
using OpenAI.Chat;

namespace GameHubApi.Providers
{
    /// <summary>
    /// Provides an implementation of <see cref="ILargeLanguageModel"/> using the OpenAI Chat API.
    /// </summary>
    public class OpenAILargeLanguageModel : ILargeLanguageModel
    {
        private readonly ChatClient chatClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAILargeLanguageModel"/> class.
        /// </summary>
        /// <param name="chatClient">The OpenAI <see cref="ChatClient"/> used to interact with the language model.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="chatClient"/> is null.</exception>
        public OpenAILargeLanguageModel(ChatClient chatClient)
        {
            this.chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
        }

        /// <summary>
        /// Generates a response from the language model based on the provided query and system instructions.
        /// </summary>
        /// <param name="query">The input query containing user prompt and system instructions.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated response message.</returns>
        public async Task<GenerateResponseResult> GenerateResponseAsync(GenerateResponseQuery query)
        {
            var response = await this.chatClient.CompleteChatAsync(new ChatMessage[]
            {
                new SystemChatMessage(query.Instructions),
                new UserChatMessage(query.Query)
            });

            return new GenerateResponseResult
            {
                Message = response?.Value?.Content?.FirstOrDefault()?.Text ?? string.Empty
            };
        }
    }
}

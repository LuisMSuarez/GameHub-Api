using GameHubApi.Providers.Contracts;
using OpenAI.Chat;

namespace GameHubApi.Providers
{
    public class OpenAILargeLanguageModel : ILargeLanguageModel
    {
        private readonly ChatClient chatClient;

        public OpenAILargeLanguageModel(
            ChatClient chatClient)
        {
            this.chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
        }

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
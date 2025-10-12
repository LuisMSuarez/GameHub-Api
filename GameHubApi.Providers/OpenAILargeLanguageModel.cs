using GameHubApi.Providers.Contracts;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

namespace GameHubApi.Providers
{
    public class OpenAILargeLanguageModel : ILargeLanguageModel
    {
        private const string SecretName = "OpenAIKey";
        private readonly string apiKey;

        public OpenAILargeLanguageModel(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var apiKeyValue = configuration[SecretName];
            this.apiKey = apiKeyValue ?? throw new ArgumentNullException(nameof(apiKeyValue));

        }
        public async Task<GenerateResponseResult> GenerateResponseAsync(GenerateResponseQuery query)
        {
            var openAIClient = new OpenAIClient(apiKey);

            var chat = new ChatClient(model: "gpt-4o-mini", apiKey: apiKey);

            var response = await chat.CompleteChatAsync(new ChatMessage[]
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
using GameHubApi.Contracts;
using System.Text;
using System.Text.Json;

namespace GameHubApi.Providers
{
    public class AzureTranslatorApi : ITranslator
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "https://api.cognitive.microsofttranslator.com";
        private const string ApiKeySecretName = "AzureTranslatorApiKey";
        private const string ApiLocationSecretName = "AzureTranslatorApiKey";
        private readonly string location;
        private readonly string apiKey;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AzureTranslatorApi(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            if (httpClientFactory == null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            var apiKeyValue = configuration[ApiKeySecretName];
            this.apiKey = apiKeyValue ?? throw new ArgumentNullException(nameof(apiKeyValue));
            var locationValue = configuration[ApiLocationSecretName];
            this.location = locationValue ?? throw new ArgumentNullException(nameof(locationValue));
            this.httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> Translate(string text, string from, string to)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text), "Text to translate cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(from))
            {
                throw new ArgumentNullException(nameof(from), "Source language cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentNullException(nameof(to), "Target language cannot be null or empty.");
            }

            var route = $"/translate?api-version=3.0&from={from}&to={to}";
            var body = new object[] { new { Text = text } };
            var requestBody = JsonSerializer.Serialize(body);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{BaseUrl}{route}"),
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Ocp-Apim-Subscription-Key", this.apiKey);
            request.Headers.Add("Ocp-Apim-Subscription-Region", this.location);
            request.Headers.Add("X-ClientTraceId", Guid.NewGuid().ToString());
            var response = await this.httpClient.SendAsync(request);
            if (response == null)
            {
                throw new InvalidOperationException("Response from Azure Translator API is null.");
            }
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Translation failed with status code {response.StatusCode}: {response.ReasonPhrase}");
            }
            var jsonResponse = response.Content.ReadAsStringAsync().Result;
            var translationResult = JsonSerializer.Deserialize<List<TranslationResult>>(jsonResponse);
            if (translationResult == null ||
                !translationResult.Any() ||
                translationResult[0].Translations.Any() ||
                string.IsNullOrWhiteSpace(translationResult[0].Translations[0].Text))
            {
                throw new InvalidOperationException("Translation result is empty or null.");
            }
            return translationResult[0].Translations[0].Text!;
        }
    }
}

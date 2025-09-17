namespace GameHubApi.Providers
{
    using GameHubApi.Providers.Exceptions;
    using Microsoft.Extensions.Configuration;
    using System.Text;
    using System.Text.Json;

    public class AzureTranslatorApi : ITranslator
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "https://api.cognitive.microsofttranslator.com";
        private const string ApiKeySecretName = "AzureTranslatorApiKey";
        private const string ApiLocationSecretName = "AzureTranslatorApiLocation";
        private readonly string location;
        private readonly string apiKey;

        public AzureTranslatorApi(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            if (httpClientFactory == null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var apiKeyValue = configuration[ApiKeySecretName];
            this.apiKey = apiKeyValue ?? throw new ArgumentNullException(nameof(apiKeyValue));
            var locationValue = configuration[ApiLocationSecretName];
            this.location = locationValue ?? throw new ArgumentNullException(nameof(locationValue));
            this.httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> Translate(string text, string? from, string to)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text), "Text to translate cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentNullException(nameof(to), "Target language cannot be null or empty.");
            }

            var route = $"/translate?api-version=3.0&to={to}";

            // from is an optional parameter, so we only add it if it's provided
            if (!string.IsNullOrWhiteSpace(from))
            {
                route += $"&from={from}";
            }

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
                throw new ProviderException(
                    ProviderResultCode.DataAccessError,
                    "Response from Azure Translator API is null.");
            }
            if (!response.IsSuccessStatusCode)
            {
                throw new ProviderException(
                    ProviderResultCode.InternalServerError,
                    $"Translation failed with status code {response.StatusCode}: {response.ReasonPhrase}");
            }
            var jsonResponse = response.Content.ReadAsStringAsync().Result;
            var translationResult = JsonSerializer.Deserialize<List<TranslationResult>>(jsonResponse);
            if (translationResult == null ||
                translationResult.Count < 1)
            {
                throw new ProviderException(
                    ProviderResultCode.InternalServerError,
                    "Translation result is empty or null.");
            }
            return translationResult.Single().Translations.First().Text;
        }
    }
}

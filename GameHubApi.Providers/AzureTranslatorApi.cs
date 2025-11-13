namespace GameHubApi.Providers
{
    using GameHubApi.Providers.Exceptions;
    using Microsoft.Extensions.Configuration;
    using System.Text;
    using System.Text.Json;

    /// <summary>
    /// Provides translation services using the Azure Cognitive Services Translator API.
    /// </summary>
    public class AzureTranslatorApi : ITranslator
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "https://api.cognitive.microsofttranslator.com";
        private const string ApiKeySecretName = "AzureTranslatorApiKey";
        private const string ApiLocationSecretName = "AzureTranslatorApiLocation";
        private readonly string location;
        private readonly string apiKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTranslatorApi"/> class.
        /// </summary>
        /// <param name="httpClientFactory">Factory to create <see cref="HttpClient"/> instances.</param>
        /// <param name="configuration">Configuration containing API key and location.</param>
        /// <exception cref="ArgumentNullException">Thrown when dependencies or configuration values are missing.</exception>
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

        /// <summary>
        /// Translates the specified text from a source language to a target language using Azure Translator.
        /// </summary>
        /// <param name="text">The text to translate.</param>
        /// <param name="from">The source language code (optional).</param>
        /// <param name="to">The target language code.</param>
        /// <returns>The translated text.</returns>
        /// <exception cref="ArgumentNullException">Thrown when required parameters are null or empty.</exception>
        /// <exception cref="ProviderException">Thrown when the translation API fails or returns invalid data.</exception>
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

            // Add source language if provided
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

            // Add required headers
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
                translationResult.Count < 1 ||
                translationResult.First().Translations == null ||
                translationResult.First().Translations.Count < 1)
            {
                throw new ProviderException(
                    ProviderResultCode.InternalServerError,
                    "Translation result is empty or null.");
            }

            return translationResult.First().Translations.First().Text;
        }
    }
}

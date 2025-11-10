namespace GameHubApi.Providers.Contracts
{
    /// <summary>
    /// Represents the result of a response generation operation from a language model.
    /// Contains the generated message content.
    /// </summary>
    public class GenerateResponseResult
    {
        /// <summary>
        /// The generated message returned by the model.
        /// </summary>
        public required string Message { get; set; }
    }
}

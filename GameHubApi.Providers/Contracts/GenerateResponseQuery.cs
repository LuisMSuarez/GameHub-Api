namespace GameHubApi.Providers.Contracts
{
    /// <summary>
    /// Represents a request payload for generating a response using a language model.
    /// Used to encapsulate the query, optional instructions, and output constraints.
    /// </summary>
    public class GenerateResponseQuery
    {
        /// <summary>
        /// The main prompt or query to be processed by the language model.
        /// </summary>
        public required string Query { get; set; }

        /// <summary>
        /// Optional instructions to guide the response generation (e.g., tone, format, constraints).
        /// </summary>
        public string? Instructions { get; set; }

        /// <summary>
        /// Maximum number of tokens allowed in the generated output.
        /// Defaults to 500 to balance verbosity and performance.
        /// </summary>
        public int MaxOutputTokens { get; set; } = 500;
    }
}

namespace GameHubApi.Providers
{
    /// <summary>
    /// Defines a contract for translating text between languages.
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        /// Translates the specified text from a source language to a target language.
        /// </summary>
        /// <param name="text">The text to translate.</param>
        /// <param name="from">The source language code (optional). If null, the language will be auto-detected.</param>
        /// <param name="to">The target language code.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the translated text.</returns>
        Task<string> Translate(string text, string? from, string to);
    }
}

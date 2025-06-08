namespace GameHubApi.Providers
{
    public interface ITranslator
    {
        Task<string> Translate(string text, string? from, string to);
    }
}

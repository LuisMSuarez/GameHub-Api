namespace GameHubApi.Providers
{
    public interface ITranslator
    {
        string Translate(string text, string from, string to);
    }
}

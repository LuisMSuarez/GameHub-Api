namespace GameHubApi.Providers.Contracts
{
    public class GenerateResponseQuery
    {
        public required string Query { get; set; }
        public string? Instructions { get; set; }
        public int MaxOutputTokens { get; set; } = 500;
    }
}

namespace GameHubApi.Providers
{
    using GameHubApi.Contracts;
    public interface IRawgApi
    {
        public Task<CollectionResult<Game>> GetGamesAsync(string? genres, string? parentPlatforms, string? ordering, string? search, int page = 1, int pageSize = 20);
        public Task<CollectionResult<Genre>> GetGenresAsync(int page = 1, int pageSize = 20);
        public Task<Game> GetGameAsync(string slug);
    }
}

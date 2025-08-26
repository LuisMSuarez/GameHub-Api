namespace GameHubApi.Services
{
    using GameHubApi.Contracts;
    public interface IGamesService
    {
        Task<CollectionResult<Game>> GetGamesAsync (string? genres, string? parentPlatforms, string? ordering, string? search, int page, int pageSize);
        Task<Game> GetGameAsync(string gameId, string? language);
        Task<CollectionResult<Movie>> GetMovies(string gameId);
        Task<CollectionResult<Screenshot>> GetScreenshots(string gameId);
    }
}

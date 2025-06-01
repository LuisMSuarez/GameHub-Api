namespace GameHubApi.Services
{
    using GameHubApi.Contracts;
    public interface IGamesService
    {
        Task<CollectionResult<Game>> GetGamesAsync (string? genres, string? parentPlatforms, string? ordering, string? search, int page, int pageSize);
        Task<Game> GetGameAsync(string id);

        Task<CollectionResult<Movie>> GetMovies(string id);
    }
}

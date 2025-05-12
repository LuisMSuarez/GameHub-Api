namespace GameHubApi.Services
{
    using GameHubApi.Contracts;
    public interface IGenresService
    {
        Task<CollectionResult<Genre>> GetGenresAsync(int page, int pageSize);
    }
}

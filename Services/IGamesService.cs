using GameHubApi.Contracts;

namespace GameHubApi.Services
{
    public interface IGamesService
    {
        Task<CollectionResult<Game>> GetGamesAsync ();
    }
}

using GameHubApi.Contracts;

namespace GameHubApi.Services
{
    public interface IGameFilter
    {
        Task<FilterResult> FilterAsync(Game game);
    }
}

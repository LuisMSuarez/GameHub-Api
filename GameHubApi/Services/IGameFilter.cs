using GameHubApi.Contracts;

namespace GameHubApi.Services
{
    public interface IGameFilter
    {
        FilterResult Filter(Game game);
    }
}

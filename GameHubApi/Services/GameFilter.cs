using GameHubApi.Contracts;

namespace GameHubApi.Services
{
    public class GameFilter : IGameFilter
    {
        public FilterResult Filter(Game game)
        {
            return FilterResult.Passed;
        }
    }
}

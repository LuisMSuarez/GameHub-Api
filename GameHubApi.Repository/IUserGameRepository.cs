using GameHubApi.Contracts;

namespace GameHubApi.Repository
{
    public interface IUserGameRepository
    {
        Task<UserGame?> GetUserGame(string id, string userId);
        Task<UserGame> CreateOrUpdateUserGamePreference(string userId, string gameId, Preference preference);
    }
}

using GameHubApi.Contracts;

namespace GameHubApi.Services
{
    public interface IUserGameService
    {
        Task<UserGame?> GetUserGame(string id, string userId);
        Task<UserGame> CreateOrUpdateUserGamePreference(string userid, string gameId, Preference preference);
    }
}

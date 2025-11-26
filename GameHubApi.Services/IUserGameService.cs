using GameHubApi.Contracts;

namespace GameHubApi.Services
{
    public interface IUserGameService
    {
        Task<UserGame> CreateOrUpdateUserGamePreference(string userid, string gameId, Preference preference);
    }
}

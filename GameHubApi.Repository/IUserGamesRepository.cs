using GameHubApi.Repository.Contracts;

namespace GameHubApi.Repository
{
    public interface IUserGamesRepository
    {
        Task<UserGame> CreateOrUpdateUserGamePreference(string userid, string gameId, Preference preference);
    }
}

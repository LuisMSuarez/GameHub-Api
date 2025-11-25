using GameHubApi.Repository.Contracts;

namespace GameHubApi.Repository
{
    public interface IUserGameRepository
    {
        Task<UserGame> CreateOrUpdateUserGamePreference(string userid, string gameId, Preference preference);
    }
}

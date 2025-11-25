using GameHubApi.Repository.Contracts;

namespace GameHubApi.Repository
{
    public class UserGameRepository : IUserGameRepository
    {
        public Task<UserGame> CreateOrUpdateUserGamePreference(string userid, string gameId, Preference preference)
        {
            throw new NotImplementedException();
        }
    }
}

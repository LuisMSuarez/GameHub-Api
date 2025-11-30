using GameHubApi.Contracts;
using Microsoft.Azure.Cosmos;

namespace GameHubApi.Services
{
    public interface IUserGameService
    {
        Task<UserGame?> GetUserGame(string id, string userId);
        Task<UserGame> UpdateUserGame(string id, string userId, UserGame userGame);
        Task<CollectionResult<UserGame>> GetUserGames(string userId);
        Task<UserGame> CreateOrUpdateUserGamePreference(string userid, string gameId, Preference preference);
    }
}

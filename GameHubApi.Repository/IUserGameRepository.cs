using GameHubApi.Contracts;

namespace GameHubApi.Repository
{
    public interface IUserGameRepository
    {
        Task<UserGame?> GetUserGame(string id, string userId);
        Task<UserGame> CreateUserGame(string userId, UserGame userGame);
        Task<UserGame> UpdateUserGame(string id, string userId, UserGame userGame);
        Task<CollectionResult<UserGame>> GetUserGames(string userId);
    }
}

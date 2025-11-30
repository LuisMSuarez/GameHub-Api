using GameHubApi.Contracts;
using GameHubApi.Repository;
using GameHubApi.Repository.Contracts;
using GameHubApi.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameHubApi.Services
{
    public class UserGameService : IUserGameService
    {
        private readonly IUserGameRepository repository;
        public UserGameService(IUserGameRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<UserGame?> GetUserGame(string id, string userId)
        {
            return this.repository.GetUserGame(id, userId);
        }

        public Task<UserGame> UpdateUserGame(string id, string userId, UserGame userGame)
        {
            if (!string.Equals(userGame.Id, id, StringComparison.OrdinalIgnoreCase))
            {
                throw new ServiceException(ServiceResultCode.BadRequest, "UserGame Id does not match the provided id.");
            }

            if (!string.Equals(userGame.UserId, userId, StringComparison.OrdinalIgnoreCase))
            {
                throw new ServiceException(ServiceResultCode.BadRequest, "UserGame UserId does not match the provided userId.");
            }

            return this.repository.UpdateUserGame(id, userId, userGame);
        }

        public Task<CollectionResult<UserGame>> GetUserGames(string userId)
        {
            return this.repository.GetUserGames(userId);
        }

        public Task<UserGame> CreateOrUpdateUserGamePreference(string userid, string gameId, Preference preference)
        {
            return this.repository.CreateOrUpdateUserGamePreference(userid, gameId, preference);
        }
    }
}

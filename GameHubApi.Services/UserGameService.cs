using GameHubApi.Contracts;
using GameHubApi.Repository;
using GameHubApi.Repository.Contracts;
using GameHubApi.Services.Exceptions;

namespace GameHubApi.Services
{
    public class UserGameService : IUserGameService
    {
        private readonly IUserGameRepository repository;

        public UserGameService(IUserGameRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<UserGame?> GetUserGame(string id, string userId)
        {
            try
            {
                return await this.repository.GetUserGame(id, userId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(MapResultCode(ex.ResultCode), ex.Message, ex);
            }
        }

        public async Task<UserGame> CreateUserGame(string userId, UserGame userGame)
        {
            if (!string.Equals(userGame.UserId, userId, StringComparison.OrdinalIgnoreCase))
            {
                throw new ServiceException(ServiceResultCode.BadRequest, "UserGame UserId does not match the provided userId.");
            }

            try
            {
                return await this.repository.CreateUserGame(userId, userGame);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(MapResultCode(ex.ResultCode), ex.Message, ex);
            }
        }

        public async Task<UserGame> UpdateUserGame(string id, string userId, UserGame userGame)
        {
            if (!string.Equals(userGame.Id, id, StringComparison.OrdinalIgnoreCase))
            {
                throw new ServiceException(ServiceResultCode.BadRequest, "UserGame Id does not match the provided id.");
            }

            if (!string.Equals(userGame.UserId, userId, StringComparison.OrdinalIgnoreCase))
            {
                throw new ServiceException(ServiceResultCode.BadRequest, "UserGame UserId does not match the provided userId.");
            }

            try
            {
                return await this.repository.UpdateUserGame(id, userId, userGame);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(MapResultCode(ex.ResultCode), ex.Message, ex);
            }
        }

        public async Task<CollectionResult<UserGame>> GetUserGames(string userId)
        {
            try
            {
                return await this.repository.GetUserGames(userId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(MapResultCode(ex.ResultCode), ex.Message, ex);
            }
        }

        /// <summary>
        /// Maps repository result codes to service result codes.
        /// </summary>
        private static ServiceResultCode MapResultCode(RepositoryResultCode repoCode)
        {
            return repoCode switch
            {
                RepositoryResultCode.BadRequest => ServiceResultCode.BadRequest,
                RepositoryResultCode.NotFound => ServiceResultCode.NotFound,
                RepositoryResultCode.Conflict => ServiceResultCode.Conflict,
                RepositoryResultCode.Forbidden => ServiceResultCode.Forbidden,
                RepositoryResultCode.InternalServerError => ServiceResultCode.InternalServerError,
                _ => ServiceResultCode.InternalServerError
            };
        }
    }
}

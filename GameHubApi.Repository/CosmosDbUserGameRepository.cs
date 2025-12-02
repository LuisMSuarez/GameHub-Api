using GameHubApi.Contracts;
using GameHubApi.Repository.Contracts;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace GameHubApi.Repository
{
    public class CosmosDbUserGameRepository : IUserGameRepository
    {
        private readonly Container container;

        public CosmosDbUserGameRepository(CosmosClient client, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            var database = client.GetDatabase(configuration["CosmosDbDatabase"]);
            this.container = database.GetContainer("userGames");
        }

        public async Task<UserGame?> GetUserGame(string id, string userId)
        {
            ValidateId(id);
            ValidateUserId(userId);

            try
            {
                var response = await this.container.ReadItemAsync<Entities.UserGame>(id, new PartitionKey(userId));
                return ToUserGameContract(response.Resource);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                throw new RepositoryException(RepositoryResultCode.InternalServerError, "Error fetching user game.", ex);
            }
        }

        public async Task<UserGame> CreateUserGame(string userId, UserGame userGame)
        {
            ValidateUserId(userId);
            ArgumentNullException.ThrowIfNull(userGame, nameof(userGame));

            if (userGame.Id != null)
            {
                throw new RepositoryException(RepositoryResultCode.BadRequest, "UserGame Id must not be provided on create.");
            }

            if (!string.Equals(userGame.UserId, userId, StringComparison.OrdinalIgnoreCase))
            {
                throw new RepositoryException(RepositoryResultCode.BadRequest, "UserGame UserId does not match the provided userId.");
            }

            try
            {
                userGame.Id = Guid.NewGuid().ToString();
                var repoEntity = ToUserGameRepository(userGame);

                var response = await this.container.CreateItemAsync(repoEntity, new PartitionKey(userId));
                return ToUserGameContract(response.Resource);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                throw new RepositoryException(RepositoryResultCode.Conflict,
                    $"UserGame for GameId {userGame.GameId} could not be created for user {userId}.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(RepositoryResultCode.InternalServerError, "Error creating user game.", ex);
            }
        }

        public async Task<UserGame> UpdateUserGame(string id, string userId, UserGame userGame)
        {
            ValidateId(id);
            ValidateUserId(userId);
            ArgumentNullException.ThrowIfNull(userGame, nameof(userGame));

            if (!string.Equals(userGame.Id, id, StringComparison.OrdinalIgnoreCase))
            {
                throw new RepositoryException(RepositoryResultCode.BadRequest, "UserGame Id does not match the provided id.");
            }

            if (!string.Equals(userGame.UserId, userId, StringComparison.OrdinalIgnoreCase))
            {
                throw new RepositoryException(RepositoryResultCode.BadRequest, "UserGame UserId does not match the provided userId.");
            }

            try
            {
                var existing = await this.container.ReadItemAsync<Entities.UserGame>(id, new PartitionKey(userId));

                if (!string.Equals(existing.Resource.UserId, userId, StringComparison.OrdinalIgnoreCase))
                    throw new RepositoryException(RepositoryResultCode.Forbidden,
                        $"UserGame with id {id} is not owned by user {userId}.");

                var repoEntity = ToUserGameRepository(userGame);
                var response = await this.container.ReplaceItemAsync(repoEntity, id, new PartitionKey(userId));

                return ToUserGameContract(response.Resource);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new RepositoryException(RepositoryResultCode.NotFound,
                    $"UserGame with id {id} not found for user {userId}.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(RepositoryResultCode.InternalServerError, "Error updating user game.", ex);
            }
        }

        public async Task<CollectionResult<UserGame>> GetUserGames(string userId)
        {
            ValidateUserId(userId);

            try
            {
                var query = new QueryDefinition("SELECT * FROM c");
                var options = new QueryRequestOptions { PartitionKey = new PartitionKey(userId) };

                var iterator = this.container.GetItemQueryIterator<Entities.UserGame>(query, requestOptions: options);
                var results = new List<UserGame>();

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    results.AddRange(response.Resource.Select(ToUserGameContract));
                }

                return new CollectionResult<UserGame>
                {
                    Count = results.Count,
                    Results = results
                };
            }
            catch (Exception ex)
            {
                throw new RepositoryException(RepositoryResultCode.InternalServerError, "Error fetching user games.", ex);
            }
        }

        private static void ValidateId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new RepositoryException(RepositoryResultCode.BadRequest, "Id is null or empty.");
            }
        }

        private static void ValidateUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new RepositoryException(RepositoryResultCode.BadRequest, "UserId is null or empty.");
            }
        }

        private static UserGame ToUserGameContract(Entities.UserGame resource) => new UserGame
        {
            Id = resource.Id,
            Slug = resource.Slug,
            Name = resource.Name,
            BackgroundImage = resource.BackgroundImage,
            GameId = resource.GameId,
            UserId = resource.UserId,
            Preferences = (Preference)resource.Preferences
        };

        private static Entities.UserGame ToUserGameRepository(UserGame contract) => new Entities.UserGame
        {
            Id = contract.Id!,
            Slug = contract.Slug,
            Name = contract.Name,
            BackgroundImage = contract.BackgroundImage,
            GameId = contract.GameId,
            UserId = contract.UserId,
            Preferences = (Entities.Preference)contract.Preferences
        };
    }
}

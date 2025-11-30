using GameHubApi.Contracts;
using GameHubApi.Repository.Contracts;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace GameHubApi.Repository
{
    public class CosmosDbUserGameRepository : IUserGameRepository
    {
        private readonly CosmosClient client;
        private readonly Container container;

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDbUrlShortcutRepository"/> class.
        /// </summary>
        /// <param name="client">The Cosmos DB client instance.</param>
        /// <param name="configurationOptions">Configuration options containing database and container names.</param>
        public CosmosDbUserGameRepository(
            CosmosClient client,
            IConfiguration configuration)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            var database = client.GetDatabase(configuration["CosmosDbDatabase"]);
            this.container = database.GetContainer("userGames");
        }

        public async Task<GameHubApi.Contracts.UserGame?> GetUserGame(string id, string userId)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new RepositoryException(RepositoryResultCode.BadRequest, "Id is null or empty.");
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new RepositoryException(RepositoryResultCode.BadRequest, "UserId is null or empty.");
            }

            try
            {
                var response = await this.container.ReadItemAsync<GameHubApi.Repository.Contracts.UserGame>(id, new PartitionKey(userId));
                return ToUserGameContract(response.Resource);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                throw new RepositoryException(RepositoryResultCode.InternalServerError, "Error fetching url shortcut.", ex);
            }
        }

        public async Task<GameHubApi.Contracts.UserGame> UpdateUserGame(
            string id,
            string userId,
            GameHubApi.Contracts.UserGame userGame)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new RepositoryException(RepositoryResultCode.BadRequest, "Id is null or empty.");
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new RepositoryException(RepositoryResultCode.BadRequest, "UserId is null or empty.");
            }

            if (userGame == null)
            {
                throw new RepositoryException(RepositoryResultCode.BadRequest, "UserGame is null.");
            }

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
                // First, read the existing item to verify ownership
                var existing = await this.container.ReadItemAsync<GameHubApi.Repository.Contracts.UserGame>(
                    id,
                    new PartitionKey(userId));

                if (!string.Equals(existing.Resource.UserId, userId, StringComparison.OrdinalIgnoreCase))
                {
                    throw new RepositoryException(
                        RepositoryResultCode.Forbidden,
                        $"UserGame with id {id} is not owned by user {userId}.");
                }

                // Map contract to repository entity
                var repoEntity = ToUserGameRepository(userGame);

                // Replace the existing item
                var response = await this.container.ReplaceItemAsync(
                    repoEntity,
                    id,
                    new PartitionKey(userId));

                return ToUserGameContract(response.Resource);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new RepositoryException(
                    RepositoryResultCode.NotFound,
                    $"UserGame with id {id} not found for user {userId}.",
                    ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(
                    RepositoryResultCode.InternalServerError,
                    "Error updating user game.",
                    ex);
            }
        }

        public async Task<CollectionResult<GameHubApi.Contracts.UserGame>> GetUserGames(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new RepositoryException(RepositoryResultCode.BadRequest, "UserId is null or empty.");
            }

            try
            {
                var query = new QueryDefinition("SELECT * FROM c");
                var options = new QueryRequestOptions { PartitionKey = new PartitionKey(userId) };

                var response = await this.container.GetItemQueryIterator<GameHubApi.Repository.Contracts.UserGame>(
                    query, requestOptions: options).ReadNextAsync();

                var results = response.Resource.ToList().Select( r => ToUserGameContract(r));
                return new CollectionResult<GameHubApi.Contracts.UserGame>
                {
                    Count = results.Count(),
                    Results = results.ToList()
                };
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new CollectionResult<GameHubApi.Contracts.UserGame>
                {
                    Count = 0
                };
            }
            catch (Exception ex)
            {
                throw new RepositoryException(RepositoryResultCode.InternalServerError, "Error fetching url shortcut.", ex);
            }
        }

        public Task<GameHubApi.Contracts.UserGame> CreateOrUpdateUserGamePreference(string userId, string gameId, GameHubApi.Contracts.Preference preference)
        {
            throw new NotImplementedException();
        }

        private GameHubApi.Contracts.UserGame ToUserGameContract(Contracts.UserGame resource)
        {
            return new GameHubApi.Contracts.UserGame
            {
                Id = resource.Id,
                Slug = resource.Slug,
                Name = resource.Name,
                BackgroundImage = resource.BackgroundImage,
                GameId = resource.GameId,
                UserId = resource.UserId,
                Preferences = (GameHubApi.Contracts.Preference)resource.Preferences
            };
        }

        private Contracts.UserGame ToUserGameRepository(GameHubApi.Contracts.UserGame contract)
        {
            return new Contracts.UserGame
            {
                Id = contract.Id,
                Slug = contract.Slug,
                Name = contract.Name,
                BackgroundImage = contract.BackgroundImage,
                GameId = contract.GameId,
                UserId = contract.UserId,
                Preferences = (Contracts.Preference)contract.Preferences
            };
        }
    }
}

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

        public Task<GameHubApi.Contracts.UserGame> CreateOrUpdateUserGamePreference(string userId, string gameId, GameHubApi.Contracts.Preference preference)
        {
            throw new NotImplementedException();
        }

        private GameHubApi.Contracts.UserGame ToUserGameContract(Contracts.UserGame resource)
        {
            return new GameHubApi.Contracts.UserGame
            {
                GameId = resource.GameId,
                UserId = resource.UserId,
                Preference = (GameHubApi.Contracts.Preference)resource.Preference
            };
        }
    }
}

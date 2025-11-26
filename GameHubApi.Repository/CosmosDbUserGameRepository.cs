using GameHubApi.Contracts;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

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

        public Task<UserGame> CreateOrUpdateUserGamePreference(string userid, string gameId, Preference preference)
        {
            throw new NotImplementedException();
        }
    }
}

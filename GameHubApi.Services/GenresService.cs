namespace GameHubApi.Services
{
    using GameHubApi.Contracts;
    using GameHubApi.Providers;
    using System.Threading.Tasks;

    public class GenresService : IGenresService
    {
        private readonly IRawgApi rawgApi;

        public GenresService(IRawgApi rawgApi)
        {
            this.rawgApi = rawgApi ?? throw new ArgumentNullException(nameof(rawgApi));
        }
        public Task<CollectionResult<Genre>> GetGenresAsync(int page, int pageSize)
        {
            return this.rawgApi.GetGenresAsync(page, pageSize);
        }
    }
}
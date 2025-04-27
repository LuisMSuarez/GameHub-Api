using GameHubApi.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GameHubApi.Services
{
    public interface IGamesService
    {
        Task<CollectionResult<Game>> GetGamesAsync (string? genres, string? parentPlatforms, string? ordering, string? search);
    }
}

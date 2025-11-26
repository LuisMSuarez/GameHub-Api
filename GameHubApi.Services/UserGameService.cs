using GameHubApi.Contracts;
using GameHubApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameHubApi.Services
{
    internal class UserGameService : IUserGameService
    {
        private readonly IUserGameRepository repository;
        public UserGameService(IUserGameRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<UserGame> CreateOrUpdateUserGamePreference(string userid, string gameId, Preference preference)
        {
            return this.repository.CreateOrUpdateUserGamePreference(userid, gameId, preference);
        }
    }
}

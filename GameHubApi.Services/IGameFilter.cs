using GameHubApi.Contracts;

namespace GameHubApi.Services
{
    /// <summary>
    /// Defines a contract for evaluating whether a game is suitable for inclusion on the platform.
    /// </summary>
    public interface IGameFilter
    {
        /// <summary>
        /// Evaluates the specified game and determines if it should be blocked or passed.
        /// </summary>
        /// <param name="game">The game to evaluate.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="FilterResult"/>
        /// indicating whether the game is <c>Blocked</c> or <c>Passed</c>.
        /// </returns>
        Task<FilterResult> FilterAsync(Game game);
    }
}

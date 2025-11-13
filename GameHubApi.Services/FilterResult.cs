namespace GameHubApi.Services
{
    /// <summary>
    /// Represents the result of evaluating whether a game is suitable for platform inclusion.
    /// </summary>
    public enum FilterResult
    {
        /// <summary>
        /// Indicates the game was flagged as inappropriate and should be blocked from the platform.
        /// </summary>
        Blocked,

        /// <summary>
        /// Indicates the game passed moderation and is suitable for inclusion on the platform.
        /// </summary>
        Passed,
    }
}

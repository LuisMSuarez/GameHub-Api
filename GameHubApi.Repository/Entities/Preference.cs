namespace GameHubApi.Repository.Contracts
{
    [Flags]
    internal enum Preference
    {
        Neutral,
        Like,
        Dislike,
        Owned,
        WishList
    }
}

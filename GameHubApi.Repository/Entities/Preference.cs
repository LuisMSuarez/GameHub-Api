namespace GameHubApi.Repository.Contracts
{
    [Flags]
    internal enum Preference
    {
        Like,
        Dislike,
        Owned,
        WishList
    }
}

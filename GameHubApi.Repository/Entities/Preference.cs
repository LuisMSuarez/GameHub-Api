namespace GameHubApi.Repository.Contracts
{
    [Flags]
    internal enum Preference
    {
        None,
        Like,
        Dislike,
        Owned,
        WishList
    }
}

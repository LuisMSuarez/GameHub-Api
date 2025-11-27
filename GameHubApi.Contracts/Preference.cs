namespace GameHubApi.Contracts
{
    [Flags]
    public enum Preference
    {
        None,
        Like,
        Dislike,
        Owned,
        WishList
    }
}

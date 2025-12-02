namespace GameHubApi.Repository.Entities
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

namespace GameHubApi.Providers.Exceptions
{
    public enum ProviderResultCode
    {
        Success = 0,
        NotFound,
        Unauthorized,
        BadRequest,
        Conflict,
        DataAccessError,
        InternalServerError
    }
}

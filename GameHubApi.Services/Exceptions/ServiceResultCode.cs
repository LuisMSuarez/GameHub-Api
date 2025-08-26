namespace GameHubApi.Services.Exceptions
{
    public enum ServiceResultCode
    {
        Success = 0,
        NotFound,
        Unauthorized,
        BadRequest,
        Conflict,
        DataAccessError,
        InternalServerError,
        Forbidden
    }
}

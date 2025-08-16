namespace GameHubApi.Services.Exceptions
{
    public class ServiceException(ServiceResultCode resultCode, string? message = null, Exception? innerException = null)
    : Exception(message, innerException)
    {
        public ServiceResultCode ResultCode { get; } = resultCode;
    }
}

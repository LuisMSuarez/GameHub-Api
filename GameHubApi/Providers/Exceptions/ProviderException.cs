namespace GameHubApi.Providers.Exceptions
{
    public class ProviderException(ProviderResultCode resultCode, string? message = null, Exception? innerException = null)
    : Exception(message, innerException)
    {
        public ProviderResultCode ResultCode { get; } = resultCode;
    }
}

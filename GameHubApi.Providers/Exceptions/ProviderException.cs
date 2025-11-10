namespace GameHubApi.Providers.Exceptions
{
    /// <summary>
    /// Represents an exception thrown by a provider when an operation fails.
    /// Encapsulates a <see cref="ProviderResultCode"/> to indicate the specific failure reason.
    /// </summary>
    public class ProviderException(
        ProviderResultCode resultCode,
        string? message = null,
        Exception? innerException = null)
        : Exception(message, innerException)
    {
        /// <summary>
        /// The result code that categorizes the type of provider failure.
        /// </summary>
        public ProviderResultCode ResultCode { get; } = resultCode;
    }
}

namespace GameHubApi.Repository.Contracts
{

    /// <summary>
    /// Represents an exception thrown by the reposutory layer.
    /// Encapsulates a <see cref="RepositoryResultCode"/> to indicate the nature of the failure.
    /// </summary>
    public class RepositoryException(RepositoryResultCode resultCode, string? message = null, Exception? innerException = null)
        : Exception(message, innerException)
    {
        /// <summary>
        /// Gets the result code that categorizes the type of data access failure.
        /// </summary>
        public RepositoryResultCode ResultCode { get; } = resultCode;
    }
}
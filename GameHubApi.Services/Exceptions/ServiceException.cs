namespace GameHubApi.Services.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs within a service layer, including a specific result code for classification.
    /// </summary>
    public class ServiceException : Exception
    {
        /// <summary>
        /// Gets the result code that categorizes the type of service error.
        /// </summary>
        public ServiceResultCode ResultCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        /// <param name="resultCode">The result code representing the type of error.</param>
        /// <param name="message">An optional message that describes the error.</param>
        /// <param name="innerException">An optional inner exception that caused the current exception.</param>
        public ServiceException(ServiceResultCode resultCode, string? message = null, Exception? innerException = null)
            : base(message, innerException)
        {
            ResultCode = resultCode;
        }
    }
}

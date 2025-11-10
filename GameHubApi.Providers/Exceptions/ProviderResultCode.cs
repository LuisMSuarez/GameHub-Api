namespace GameHubApi.Providers.Exceptions
{
    /// <summary>
    /// Represents standardized result codes for provider-level operations.
    /// Used to classify outcomes and guide exception handling logic.
    /// </summary>
    public enum ProviderResultCode
    {
        /// <summary>
        /// Operation completed successfully.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The requested resource could not be found.
        /// </summary>
        NotFound,

        /// <summary>
        /// The caller is not authorized to perform the operation.
        /// </summary>
        Unauthorized,

        /// <summary>
        /// The request was malformed or invalid.
        /// </summary>
        BadRequest,

        /// <summary>
        /// The operation could not be completed due to a conflict (e.g., duplicate data).
        /// </summary>
        Conflict,

        /// <summary>
        /// A failure occurred while accessing or processing data.
        /// </summary>
        DataAccessError,

        /// <summary>
        /// An unexpected server-side error occurred.
        /// </summary>
        InternalServerError
    }
}

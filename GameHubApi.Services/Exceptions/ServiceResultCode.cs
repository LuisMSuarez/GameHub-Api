namespace GameHubApi.Services.Exceptions
{
    /// <summary>
    /// Represents standardized result codes for service-level operations and exceptions.
    /// </summary>
    public enum ServiceResultCode
    {
        /// <summary>
        /// Indicates the operation completed successfully.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Indicates the requested resource was not found.
        /// </summary>
        NotFound,

        /// <summary>
        /// Indicates the caller is not authorized to perform the operation.
        /// </summary>
        Unauthorized,

        /// <summary>
        /// Indicates the request was malformed or invalid.
        /// </summary>
        BadRequest,

        /// <summary>
        /// Indicates a conflict occurred, such as a duplicate resource or invalid state.
        /// </summary>
        Conflict,

        /// <summary>
        /// Indicates an error occurred while accessing data or external resources.
        /// </summary>
        DataAccessError,

        /// <summary>
        /// Indicates an unexpected internal server error occurred.
        /// </summary>
        InternalServerError,

        /// <summary>
        /// Indicates the caller is explicitly forbidden from performing the operation.
        /// </summary>
        Forbidden
    }
}

namespace QueryTracker.Api.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Validates the supplied credentials against the database.
        /// Returns the uppercased userName on success, or null on failure.
        /// </summary>
        string? ValidateUser(string userName, string password);
    }
}

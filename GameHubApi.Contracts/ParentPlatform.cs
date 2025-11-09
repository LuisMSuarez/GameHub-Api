namespace GameHubApi.Contracts
{
    // Enables JSON property name mapping for serialization/deserialization
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a wrapper for a platform object within the GameHub API.
    /// Used to model hierarchical or grouped platform data (e.g., "PlayStation" as a parent of "PS4", "PS5").
    /// </summary>
    public class ParentPlatform
    {
        /// <summary>
        /// The platform associated with this parent grouping.
        /// Typically includes metadata like name, slug, and icon.
        /// </summary>
        [JsonPropertyName("platform")]
        public required Platform Platform { get; set; }
    }
}

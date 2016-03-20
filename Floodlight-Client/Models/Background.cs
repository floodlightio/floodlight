namespace Floodlight.Client.Models
{
    /// <summary>
    /// Metadata about a background.
    /// </summary>
    public class Background
    {
        /// <summary>
        /// The string representation of the GUID ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The title of the background.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The content type of the background.
        /// </summary>
        public string ContentType { get; set; }
    }
}

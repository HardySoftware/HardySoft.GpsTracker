namespace HardySoft.GpsTracker.Services.Gpx
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Windows.Devices.Geolocation;

    /// <summary>
    /// An interface to define GPX file handling functions.
    /// </summary>
    public interface IGpxHandler
    {
        /// <summary>
        /// Record a geo point to GPX way-point section.
        /// </summary>
        /// <param name="trackingId">The unique identifier of each individual tracking.</param>
        /// <param name="coordinate">The coordinate information to record.</param>
        /// <param name="comment">Additional comment message.</param>
        /// <returns>The asynchronous task.</returns>
        Task RecordLocationAsync(string trackingId, Geocoordinate coordinate, string comment);

        /// <summary>
        /// Record a Xml comment section.
        /// </summary>
        /// <param name="trackingId">The unique identifier of each individual tracking.</param>
        /// <param name="comment">Additional comment message.</param>
        /// <returns>The asynchronous task.</returns>
        Task RecordCommentAsync(string trackingId, string comment);

        /// <summary>
        /// Compose previously saved way-point sections belonging to same tracking Id into a complete GPX file.
        /// </summary>
        /// <param name="trackingId">The unique identifier of each individual tracking.</param>
        /// <param name="activityName">The name of activity.</param>
        /// <returns>The number of way points found for the final GPX file.</returns>
        Task<int> ComposeGpxFile(string trackingId, string activityName);

        /// <summary>
        /// Clears all temporary GPX way-point files.
        /// </summary>
        /// <returns>A pair structure, the key is the total number of files deleted and value is the total size of files deleted.</returns>
        Task<KeyValuePair<int, ulong>> ClearTemporaryGpxWaypointFiles();
    }
}

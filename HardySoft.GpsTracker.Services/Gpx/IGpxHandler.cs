namespace HardySoft.GpsTracker.Services.Gpx
{
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
        /// <returns>The asynchronous task.</returns>
        Task RecordLocationAsync(string trackingId, Geocoordinate coordinate);

        /// <summary>
        /// Compose previously saved way-point sections belonging to same tracking Id into a complete GPX file.
        /// </summary>
        /// <param name="trackingId">The unique identifier of each individual tracking.</param>
        /// <param name="activityName">The name of activity.</param>
        /// <returns>The number of way points found for the final GPX file.</returns>
        Task<int> ComposeGpxFile(string trackingId, string activityName);
    }
}

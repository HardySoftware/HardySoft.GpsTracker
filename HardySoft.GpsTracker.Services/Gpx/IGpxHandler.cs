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
        /// Record a geo point to GPX Xml section.
        /// </summary>
        /// <param name="trackingId">The unique identifier of each individual tracking.</param>
        /// <param name="coordinate">The coordinate information to record.</param>
        /// <returns>The asynchronous task.</returns>
        Task RecordLocationAsync(string trackingId, Geocoordinate coordinate);
    }
}

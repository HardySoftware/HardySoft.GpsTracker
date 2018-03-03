namespace HardySoft.GpsTracker.Services.Location
{
    using System.Threading.Tasks;
    using HardySoft.GpsTracker.Services.Gpx.Models;
    using Windows.Devices.Geolocation;

    /// <summary>
    /// An interface to define location tracker functions.
    /// </summary>
    public interface ILocationTracker
    {
        /// <summary>
        /// An event to raise when tracking status or position is changed.
        /// </summary>
        event UpdateTrackingProgress OnTrackingProgressChangedEvent;

        /// <summary>
        /// Starts location tracking.
        /// </summary>
        /// <param name="desireAccuracyInMeters">The desired accuracy in meters from the GPS.</param>
        /// <param name="reportIntervalInSeconds">The report internal in seconds when position is changed.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StartTracking(uint desireAccuracyInMeters, uint reportIntervalInSeconds);

        /// <summary>
        /// Stops location tracking.
        /// </summary>
        void StopTracking();

        /// <summary>
        /// Gets the current location for only once.
        /// </summary>
        /// <param name="desireAccuracyInMeters">The desired accuracy in meter from the GPS.</param>
        /// <returns>The coordination of current location.</returns>
        Task<Geocoordinate> GetCurrentLocation(uint desireAccuracyInMeters);
    }
}

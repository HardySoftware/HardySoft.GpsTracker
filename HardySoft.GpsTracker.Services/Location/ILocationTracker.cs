namespace HardySoft.GpsTracker.Services.Location
{
    using System.Threading.Tasks;
    using HardySoft.GpsTracker.Services.Models;

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
        /// Start location tracking.
        /// </summary>
        /// <param name="desireAccuracyInMeters">The desired accuracy in meter from the GPS.</param>
        /// <param name="reportIntervalInSeconds">The report internal in seconds when position is changed.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StartTracking(uint desireAccuracyInMeters, uint reportIntervalInSeconds);

        /// <summary>
        /// Stop location tracking.
        /// </summary>
        void StopTracking();
    }
}

namespace HardySoft.GpsTracker.Models
{
    using System.ComponentModel;

    /// <summary>
    /// An enumeration to define the possible implementation of location tracking mechanism.
    /// </summary>
    public enum TrackingMechanism
    {
        /// <summary>
        /// Tracks location by using location service's progress changed event.
        /// </summary>
        [Description("Using location service progress changed event.")]
        LocationServiceProgressChangedEvent = 1,

        /// <summary>
        /// Tracks location by using timer to fetch current information.
        /// </summary>
        [Description("Using a timer to control fetching intervals.")]
        LocationFetchingTimer = 2,

        /// <summary>
        /// Tracks location primarily by location service changed event,
        /// but in case there is no such event the application uses
        /// the timer to fetch the current location.
        /// </summary>
        [Description("Using location service progress changed event, but timer to compensate.")]
        Hybrid = 4,
    }
}

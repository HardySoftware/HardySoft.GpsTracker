namespace HardySoft.GpsTracker.ViewModels
{
    /// <summary>
    /// An enumeration to define the possible implementation of location tracking mechanism.
    /// </summary>
    internal enum TrackingMechanism
    {
        /// <summary>
        /// Tracks location by using location service's progress changed event.
        /// </summary>
        LocationServiceProgrssChangedEvent,

        /// <summary>
        /// Tracks location by using timer to fetch current information.
        /// </summary>
        LocationFetchingTimer,

        /// <summary>
        /// Tracks location primarily by location service changed event,
        /// but in case there is no such event the application uses
        /// the timer to fetch the current location.
        /// </summary>
        Hybrid,
    }
}

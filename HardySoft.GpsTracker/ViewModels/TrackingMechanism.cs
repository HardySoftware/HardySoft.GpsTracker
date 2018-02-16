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
        LocationFetchingTimer
    }
}

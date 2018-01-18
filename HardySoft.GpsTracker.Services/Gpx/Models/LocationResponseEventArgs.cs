namespace HardySoft.GpsTracker.Services.Gpx.Models
{
    using System;
    using Windows.Devices.Geolocation;

    /// <summary>
    /// A model to contain location response information.
    /// </summary>
    public class LocationResponseEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the geolocation access status.
        /// </summary>
        public GeolocationAccessStatus GeolocationAccessStatus { get; set; }

        /// <summary>
        /// Gets or sets the position status.
        /// </summary>
        public PositionStatus PositionStatus { get; set; }

        /// <summary>
        /// Gets or sets the coordinate of the position if available.
        /// </summary>
        public Geocoordinate Coordinate { get; set; }
    }
}

﻿namespace HardySoft.GpsTracker.Services.Models
{
    using Windows.Devices.Geolocation;

    /// <summary>
    /// A model to contain location response information.
    /// </summary>
    public class LocationResponse
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

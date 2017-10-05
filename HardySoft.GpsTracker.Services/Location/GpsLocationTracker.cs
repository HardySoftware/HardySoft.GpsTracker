namespace HardySoft.GpsTracker.Services.Location
{
    using System;
    using System.Threading.Tasks;
    using HardySoft.GpsTracker.Services.Models;
    using Windows.Devices.Geolocation;

    /// <summary>
    /// The implementation of <see cref="ILocationTracker"/> by using GPS functions available.
    /// </summary>
    public class GpsLocationTracker : ILocationTracker
    {
        /// <summary>
        /// The location tracking status object.
        /// </summary>
        private readonly LocationResponse statusUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpsLocationTracker"/> class.
        /// </summary>
        public GpsLocationTracker()
        {
            this.statusUpdate = new LocationResponse();
        }

        /// <inheritdoc />
        public event UpdateTrackingProgress OnTrackingProgressChangedEvent;

        /// <inheritdoc />
        public async Task StartTrack(uint desireAccuracyInMeters, uint reportIntervalInSeconds)
        {
            var accessStatus = await Geolocator.RequestAccessAsync();

            this.statusUpdate.GeolocationAccessStatus = accessStatus;

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                    var geolocator = new Geolocator { DesiredAccuracyInMeters = desireAccuracyInMeters, ReportInterval = reportIntervalInSeconds * 1000 };

                    // Subscribe to the StatusChanged event to get updates of location status changes.
                    geolocator.StatusChanged += this.OnStatusChanged;
                    geolocator.PositionChanged += this.OnPositionChanged;

                    // Carry out the operation.
                    Geoposition firstPosition = await geolocator.GetGeopositionAsync();
                    this.statusUpdate.Coordinate = firstPosition.Coordinate;
                    break;
            }

            this.OnTrackingProgressChangedEvent(this.statusUpdate);
        }

        /// <summary>
        /// An event handler to respond to position changed event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event argument.</param>
        private void OnPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            this.statusUpdate.Coordinate = args.Position.Coordinate;
            this.OnTrackingProgressChangedEvent(this.statusUpdate);
        }

        /// <summary>
        /// An event handler to respond to geolocator status changed event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event argument.</param>
        private void OnStatusChanged(Geolocator sender, StatusChangedEventArgs e)
        {
            this.statusUpdate.PositionStatus = e.Status;
            this.OnTrackingProgressChangedEvent(this.statusUpdate);
        }
    }
}

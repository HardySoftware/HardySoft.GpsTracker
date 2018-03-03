namespace HardySoft.GpsTracker.Services.Location
{
    using System;
    using System.Threading.Tasks;
    using HardySoft.GpsTracker.Services.Gpx.Models;
    using Windows.Devices.Geolocation;

    /// <summary>
    /// The implementation of <see cref="ILocationTracker"/> by using GPS functions available.
    /// </summary>
    public class GpsLocationTracker : ILocationTracker
    {
        /// <summary>
        /// The location tracking status object.
        /// </summary>
        private readonly LocationResponseEventArgs statusUpdate;

        /// <summary>
        /// The Geo locator object.
        /// </summary>
        private Geolocator geoLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpsLocationTracker"/> class.
        /// </summary>
        public GpsLocationTracker()
        {
            this.statusUpdate = new LocationResponseEventArgs();
        }

        /// <inheritdoc />
        public event UpdateTrackingProgress OnTrackingProgressChangedEvent;

        /// <inheritdoc />
        public async Task StartTracking(uint desireAccuracyInMeters, uint reportIntervalInSeconds)
        {
            await this.InitializeGeolocator(desireAccuracyInMeters);

            if (this.geoLocator != null)
            {
                // Subscribe to the StatusChanged event to get updates of location status changes.
                this.geoLocator.StatusChanged += this.OnStatusChanged;
                this.geoLocator.PositionChanged += this.OnPositionChanged;

                // Carry out the operation.
                Geoposition firstPosition = await this.geoLocator.GetGeopositionAsync();
                this.statusUpdate.Coordinate = firstPosition.Coordinate;
            }
        }

        /// <inheritdoc />
        public void StopTracking()
        {
            if (this.geoLocator != null)
            {
                this.geoLocator.StatusChanged -= this.OnStatusChanged;
                this.geoLocator.PositionChanged -= this.OnPositionChanged;
            }

            this.geoLocator = null;
        }

        /// <inheritdoc />
        public async Task<Geocoordinate> GetCurrentLocation(uint desireAccuracyInMeters)
        {
            await this.InitializeGeolocator(desireAccuracyInMeters);

            if (this.geoLocator != null)
            {
                Geoposition currentPosition = await this.geoLocator.GetGeopositionAsync();
                return currentPosition.Coordinate;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Initialize Geolocator by setting desired accuracy.
        /// </summary>
        /// <param name="desireAccuracyInMeters">The desired accuracy in meters from the GPS.</param>
        /// <returns>The async task.</returns>
        private async Task InitializeGeolocator(uint desireAccuracyInMeters)
        {
            if (this.geoLocator != null)
            {
                return;
            }

            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                    this.geoLocator = new Geolocator { DesiredAccuracyInMeters = desireAccuracyInMeters };
                    return;
            }

            this.geoLocator = null;
        }

        /// <summary>
        /// An event handler to respond to position changed event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event argument.</param>
        private void OnPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            this.statusUpdate.Coordinate = args.Position.Coordinate;
            this.OnTrackingProgressChangedEvent(this, this.statusUpdate);
        }

        /// <summary>
        /// An event handler to respond to geolocator status changed event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event argument.</param>
        private void OnStatusChanged(Geolocator sender, StatusChangedEventArgs e)
        {
            /*this.statusUpdate.PositionStatus = e.Status;
            this.statusUpdate.Coordinate = null;
            this.OnTrackingProgressChangedEvent(this, this.statusUpdate);*/
            return;
        }
    }
}

namespace HardySoft.GpsTracker.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using HardySoft.GpsTracker.Models;
    using HardySoft.GpsTracker.Services.Gpx.Models;
    using HardySoft.GpsTracker.Services.Location;
    using Prism.Commands;
    using Prism.Windows.Mvvm;
    using Windows.Devices.Geolocation;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A view model for current location page.
    /// </summary>
    public class CurrentLocationPageViewModel : ViewModelBase
    {
        /// <summary>
        /// A location tracker implementation.
        /// </summary>
        private readonly ILocationTracker locationTracker;

        /// <summary>
        /// The latitude display value
        /// </summary>
        private string latitudeDisplayValue;

        /// <summary>
        /// the longitude display value
        /// </summary>
        private string longitudeDisplayValue;

        /// <summary>
        /// The accuracy display value.
        /// </summary>
        private string accuracyDisplayValue;

        /// <summary>
        /// The altitude display value.
        /// </summary>
        private string altitudeDisplayValue;

        /// <summary>
        /// The altitude accuracy display value.
        /// </summary>
        private string altitudeAccuracyDisplayValue;

        /// <summary>
        /// The heading display value.
        /// </summary>
        private string headingDisplayValue;

        /// <summary>
        /// The speed display value
        /// </summary>
        private string speedDisplayValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentLocationPageViewModel"/> class.
        /// </summary>
        /// <param name="locationTracker">The location tracker implementation it depends on.</param>
        public CurrentLocationPageViewModel(ILocationTracker locationTracker)
        {
            this.locationTracker = locationTracker ?? throw new ArgumentNullException(nameof(locationTracker));

            this.LatitudeDisplayValue = "-";
            this.LongitudeDisplayValue = "-";
            this.AccuracyDisplayValue = "-";
            this.AltitudeDisplayValue = "-";
            this.AltitudeAccuracyDisplayValue = "-";
            this.HeadingDisplayValue = "-";
            this.SpeedDisplayValue = "-";

            this.locationTracker.OnTrackingProgressChangedEvent += this.LocationTracker_OnTrackingProgressChangedEvent;
            this.StartButtonClickedCommand = new DelegateCommand<ItemClickEventArgs>(this.OnStartClicked, this.CanStartClick);
        }

        /// <summary>
        /// Gets the latitude display value.
        /// </summary>
        public string LatitudeDisplayValue
        {
            get
            {
                return this.latitudeDisplayValue;
            }

            private set
            {
                this.SetProperty(ref this.latitudeDisplayValue, value);
            }
        }

        /// <summary>
        /// Gets the longitude display value.
        /// </summary>
        public string LongitudeDisplayValue
        {
            get
            {
                return this.longitudeDisplayValue;
            }

            private set
            {
                this.SetProperty(ref this.longitudeDisplayValue, value);
            }
        }

        /// <summary>
        /// Gets the accuracy display value.
        /// </summary>
        public string AccuracyDisplayValue
        {
            get
            {
                return this.accuracyDisplayValue;
            }

            private set
            {
                this.SetProperty(ref this.accuracyDisplayValue, value);
            }
        }

        /// <summary>
        /// Gets the altitude display value.
        /// </summary>
        public string AltitudeDisplayValue
        {
            get
            {
                return this.altitudeDisplayValue;
            }

            private set
            {
                this.SetProperty(ref this.altitudeDisplayValue, value);
            }
        }

        /// <summary>
        /// Gets the latitude accuracy display value.
        /// </summary>
        public string AltitudeAccuracyDisplayValue
        {
            get
            {
                return this.altitudeAccuracyDisplayValue;
            }

            private set
            {
                this.SetProperty(ref this.altitudeAccuracyDisplayValue, value);
            }
        }

        /// <summary>
        /// Gets the heading display value.
        /// </summary>
        public string HeadingDisplayValue
        {
            get
            {
                return this.headingDisplayValue;
            }

            private set
            {
                this.SetProperty(ref this.headingDisplayValue, value);
            }
        }

        /// <summary>
        /// Gets the speed display value.
        /// </summary>
        public string SpeedDisplayValue
        {
            get
            {
                return this.speedDisplayValue;
            }

            private set
            {
                this.SetProperty(ref this.speedDisplayValue, value);
            }
        }

        /// <summary>
        /// Gets the command to handle start/pause button clicked event.
        /// </summary>
        public ICommand StartButtonClickedCommand { get; private set; }

        /// <summary>
        /// Handle start/pause button clicked event.
        /// </summary>
        /// <param name="argument">The event argument.</param>
        private async void OnStartClicked(ItemClickEventArgs argument)
        {
            await this.GetLocationData();
        }

        /// <summary>
        /// Checks if the start/pause button can be clicked.
        /// </summary>
        /// <param name="argument">The argument event.</param>
        /// <returns>True if the click is allowed.</returns>
        private bool CanStartClick(ItemClickEventArgs argument)
        {
            return true;
        }

        /// <summary>
        /// Gets the location data.
        /// </summary>
        /// <returns>The asynchronous task.</returns>
        private async Task GetLocationData()
        {
            var locationAccessStatus = await Geolocator.RequestAccessAsync();
            if (locationAccessStatus == GeolocationAccessStatus.Allowed)
            {
                await this.locationTracker.StartTracking(20, 10);
            }
        }

        /// <summary>
        /// An event handler to handle GPS track changed event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="statusUpdate">The event argument with detailed data.</param>
        private void LocationTracker_OnTrackingProgressChangedEvent(object sender, LocationResponseEventArgs statusUpdate)
        {
            if (statusUpdate.Coordinate != null)
            {
                Debug.WriteLine($"{DateTime.Now} - GPS position or status has changed.");

                this.LatitudeDisplayValue = this.GetLatitudeLongitudeDisplayValue(statusUpdate.Coordinate.Latitude, LocationPointValueType.Latitude);
                this.LongitudeDisplayValue = this.GetLatitudeLongitudeDisplayValue(statusUpdate.Coordinate.Longitude, LocationPointValueType.Longitude);
            }
        }

        /// <summary>
        /// Get latitude or longitude decimal and degree display values.
        /// </summary>
        /// <param name="value">The latitude or longitude value in decimal.</param>
        /// <param name="type">The type of the decimal value.</param>
        /// <returns>A string representing the latitude's decimal and degree values combined.</returns>
        private string GetLatitudeLongitudeDisplayValue(double value, LocationPointValueType type)
        {
            string display = value.ToString();
            var dms = new DmsPoint(value, LocationPointValueType.Latitude);
            return $"{value} / {dms.Degree}°{dms.Minute}'{dms.Second}'' {dms.Direction}";
        }
    }
}

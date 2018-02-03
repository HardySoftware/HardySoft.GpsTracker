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
    using Windows.ApplicationModel.Core;
    using Windows.Devices.Geolocation;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A view model for current location page.
    /// </summary>
    public class CurrentLocationPageViewModel : ViewModelBase
    {
        /// <summary>
        /// A symbol to represent unknown value.
        /// </summary>
        private const string UnknownValue = "-";

        /// <summary>
        /// A location tracker implementation.
        /// </summary>
        private readonly ILocationTracker locationTracker;

        /// <summary>
        /// The position source display value
        /// </summary>
        private string positionSourceDisplayValue;

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

            this.PositionSourceDisplayValue = UnknownValue;
            this.LatitudeDisplayValue = UnknownValue;
            this.LongitudeDisplayValue = UnknownValue;
            this.AccuracyDisplayValue = UnknownValue;
            this.AltitudeDisplayValue = UnknownValue;
            this.AltitudeAccuracyDisplayValue = UnknownValue;
            this.HeadingDisplayValue = UnknownValue;
            this.SpeedDisplayValue = UnknownValue;

            this.locationTracker.OnTrackingProgressChangedEvent += this.LocationTracker_OnTrackingProgressChangedEvent;
            this.StartButtonClickedCommand = new DelegateCommand<ItemClickEventArgs>(this.OnStartClicked, this.CanStartClick);
        }

        /// <summary>
        /// Gets the position source display value.
        /// </summary>
        public string PositionSourceDisplayValue
        {
            get
            {
                return this.positionSourceDisplayValue;
            }

            private set
            {
                this.SetProperty(ref this.positionSourceDisplayValue, value);
            }
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
        /// Gets latitude or longitude decimal and degree display values.
        /// </summary>
        /// <param name="value">The latitude or longitude value in decimal.</param>
        /// <param name="type">The type of the decimal value.</param>
        /// <returns>A string representing the latitude's decimal and degree values combined.</returns>
        private static string GetLatitudeLongitudeDisplayValue(double? value, LocationPointValueType type)
        {
            if (value.HasValue && !double.IsNaN(value.Value))
            {
                string display = value.Value.ToString();
                var dms = new DmsPoint(value.Value, type);
                return $"{value} / {dms.Degree}°{dms.Minute}'{dms.Second}'' {dms.Direction}";
            }
            else
            {
                return UnknownValue;
            }
        }

        /// <summary>
        /// Gets the display value in both meter and feet units.
        /// </summary>
        /// <param name="value">The value in meter.</param>
        /// <param name="unitSuffix">Additional suffix to the unit.</param>
        /// <returns>The formatted display value with both meter and feet.</returns>
        private static string GetDisplayValueForMeterFeet(double? value, string unitSuffix)
        {
            if (value.HasValue && !double.IsNaN(value.Value))
            {
                var display = $"{value:0,0.00} m{unitSuffix}";
                display += " / ";
                display += $"{value / 3.2808399:0,0.00} ft{unitSuffix}";

                return display;
            }
            else
            {
                return UnknownValue;
            }
        }

        /// <summary>
        /// Gets cardinal direction display value.
        /// </summary>
        /// <param name="degrees">Degrees relative to true north.</param>
        /// <returns>The formatted display value.</returns>
        private static string GetCardinalDirection(double? degrees)
        {
            if (degrees.HasValue && !double.IsNaN(degrees.Value))
            {
                string[] caridnals = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
                var cardinal = caridnals[(int)Math.Round(((degrees.Value * 10) % 3600) / 225)];

                return $"{cardinal} ({degrees.Value})";
            }
            else
            {
                return UnknownValue;
            }
        }

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
        private async void LocationTracker_OnTrackingProgressChangedEvent(object sender, LocationResponseEventArgs statusUpdate)
        {
            if (statusUpdate.Coordinate != null)
            {
                Debug.WriteLine($"{DateTime.Now} - GPS position or status has changed.");
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.PositionSourceDisplayValue = statusUpdate.Coordinate.PositionSource == PositionSource.Unknown ? UnknownValue : statusUpdate.Coordinate.PositionSource.ToString();
                    this.LatitudeDisplayValue = GetLatitudeLongitudeDisplayValue(statusUpdate.Coordinate.Latitude, LocationPointValueType.Latitude);
                    this.LongitudeDisplayValue = GetLatitudeLongitudeDisplayValue(statusUpdate.Coordinate.Longitude, LocationPointValueType.Longitude);
                    this.AccuracyDisplayValue = GetDisplayValueForMeterFeet(statusUpdate.Coordinate.Accuracy, string.Empty);
                    this.AltitudeDisplayValue = GetDisplayValueForMeterFeet(statusUpdate.Coordinate.Altitude, string.Empty);
                    this.AltitudeAccuracyDisplayValue = GetDisplayValueForMeterFeet(statusUpdate.Coordinate.AltitudeAccuracy, string.Empty);
                    this.HeadingDisplayValue = GetCardinalDirection(statusUpdate.Coordinate.Heading);
                    this.SpeedDisplayValue = GetDisplayValueForMeterFeet(statusUpdate.Coordinate.Speed, "/s");
                });
            }
        }
    }
}

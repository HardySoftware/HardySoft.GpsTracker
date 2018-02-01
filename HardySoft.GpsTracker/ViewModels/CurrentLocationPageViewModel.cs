namespace HardySoft.GpsTracker.ViewModels
{
    using Prism.Windows.Mvvm;

    /// <summary>
    /// A view model for current location page.
    /// </summary>
    public class CurrentLocationPageViewModel : ViewModelBase
    {
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
        public CurrentLocationPageViewModel()
        {
            this.LatitudeDisplayValue = "-";
            this.LongitudeDisplayValue = "-";
            this.AccuracyDisplayValue = "-";
            this.AltitudeDisplayValue = "-";
            this.AltitudeAccuracyDisplayValue = "-";
            this.HeadingDisplayValue = "-";
            this.SpeedDisplayValue = "-";
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
    }
}

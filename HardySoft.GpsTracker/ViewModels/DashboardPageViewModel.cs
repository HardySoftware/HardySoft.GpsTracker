namespace HardySoft.GpsTracker.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using HardySoft.GpsTracker.Models;
    using HardySoft.GpsTracker.Services.Gpx;
    using HardySoft.GpsTracker.Services.Models;
    using Prism.Mvvm;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A view model for dashboard page.
    /// </summary>
    public class DashboardPageViewModel : BindableBase
    {
        /// <summary>
        /// A GPX handler implementation.
        /// </summary>
        private readonly IGpxHandler gpxHandler;

        /// <summary>
        /// The status of the tracking.
        /// </summary>
        private TrackingStatus status;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardPageViewModel"/> class.
        /// </summary>
        /// <param name="gpxHandler">The Gpx handler implementation it depends on.</param>
        public DashboardPageViewModel(IGpxHandler gpxHandler)
        {
            this.gpxHandler = gpxHandler ?? throw new ArgumentNullException(nameof(gpxHandler));

            this.status = TrackingStatus.Stopped;
        }

        /// <summary>
        /// Gets a list of supported activity types.
        /// </summary>
        public IEnumerable<ActivityTypes> SupportedActivityTypes => Enum.GetValues(typeof(ActivityTypes)).Cast<ActivityTypes>();

        /// <summary>
        /// Gets the icon for the start/pause button.
        /// </summary>
        public Symbol StartPauseButtonIcon
        {
            get
            {
                switch (this.status)
                {
                    case TrackingStatus.Started:
                        return Symbol.Pause;
                    case TrackingStatus.Paused:
                        return Symbol.Play;
                    case TrackingStatus.Stopped:
                        return Symbol.Play;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Gets the description text for the start/pause button.
        /// </summary>
        public string StartPauseButtonDescription
        {
            get
            {
                switch (this.status)
                {
                    case TrackingStatus.Started:
                        return Symbol.Pause.ToString();
                    case TrackingStatus.Paused:
                        return Symbol.Play.ToString();
                    case TrackingStatus.Stopped:
                        return Symbol.Play.ToString();
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Gets the icon for the stop button.
        /// </summary>
        public Symbol StopButtonIcon => Symbol.Stop;
    }
}

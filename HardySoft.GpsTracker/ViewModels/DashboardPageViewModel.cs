namespace HardySoft.GpsTracker.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using HardySoft.GpsTracker.Models;
    using HardySoft.GpsTracker.Services.Gpx;
    using HardySoft.GpsTracker.Services.Location;
    using HardySoft.GpsTracker.Services.Models;
    using Prism.Commands;
    using Prism.Windows.AppModel;
    using Prism.Windows.Mvvm;
    using Windows.ApplicationModel.Core;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A view model for dashboard page.
    /// </summary>
    public class DashboardPageViewModel : ViewModelBase
    {
        /// <summary>
        /// A GPX handler implementation.
        /// </summary>
        private readonly IGpxHandler gpxHandler;

        /// <summary>
        /// A location tracker implementation.
        /// </summary>
        private readonly ILocationTracker locationTracker;

        /// <summary>
        /// The status of the tracking.
        /// </summary>
        private TrackingStatus status;

        /// <summary>
        /// The selected activity;
        /// </summary>
        private ActivityTypes selectedActivity;

        /// <summary>
        /// The text information of the coordinate from location tracking service.
        /// </summary>
        private string coordinateInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardPageViewModel"/> class.
        /// </summary>
        /// <param name="gpxHandler">The Gpx handler implementation it depends on.</param>
        /// <param name="locationTracker">The location tracker implementation it depends on.</param>
        public DashboardPageViewModel(IGpxHandler gpxHandler, ILocationTracker locationTracker)
        {
            this.gpxHandler = gpxHandler ?? throw new ArgumentNullException(nameof(gpxHandler));
            this.locationTracker = locationTracker ?? throw new ArgumentNullException(nameof(locationTracker));

            this.status = TrackingStatus.Stopped;
            this.StartPauseClickedCommand = new DelegateCommand<ItemClickEventArgs>(this.OnStartPauseClicked, this.CanStartPauseClick);
            this.SelectedActivity = ActivityTypes.Unknown;
            this.CoordinateInformation = "Your location information";

            this.locationTracker.OnTrackingProgressChangedEvent += this.LocationTracker_OnTrackingProgressChangedEvent;
        }

        /// <summary>
        /// Gets a list of activity types and their display texts.
        /// </summary>
        public ObservableCollection<ActivityTypeDisplay> SupportedActivityTypeDisplay => new ObservableCollection<ActivityTypeDisplay>(ActivityTypeDisplay.GetAllActivityTypeDisplay());

        /// <summary>
        /// Gets or sets the selected activity.
        /// </summary>
        [RestorableState]
        public ActivityTypes SelectedActivity
        {
            get
            {
                return this.selectedActivity;
            }

            set
            {
                this.SetProperty(ref this.selectedActivity, value);
            }
        }

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

        /// <summary>
        /// Gets the command to handle start/pause button clicked event.
        /// </summary>
        [RestorableState]
        public ICommand StartPauseClickedCommand { get; private set; }

        /// <summary>
        /// Gets the text information of the coordinate from location tracking service.
        /// </summary>
        [RestorableState]
        public string CoordinateInformation
        {
            get
            {
                return this.coordinateInformation;
            }

            private set
            {
                this.SetProperty(ref this.coordinateInformation, value);
            }
        }

        /// <summary>
        /// Handle start/pause button clicked event.
        /// </summary>
        /// <param name="argument">The event argument.</param>
        private async void OnStartPauseClicked(ItemClickEventArgs argument)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.CoordinateInformation = "In progress.";
            });

            await this.locationTracker.StartTrack(5, 60);
        }

        /// <summary>
        /// Checks if the start/pause button can be clicked.
        /// </summary>
        /// <param name="argument">The argument event.</param>
        /// <returns>True if the click is allowed.</returns>
        private bool CanStartPauseClick(ItemClickEventArgs argument)
        {
            // return this.SelectedActivity == null ? false : true;
            return true;
        }

        /// <summary>
        /// An event handler to handle GPS track changed event.
        /// </summary>
        /// <param name="statusUpdate">The event argument with detailed data.</param>
        private async void LocationTracker_OnTrackingProgressChangedEvent(LocationResponse statusUpdate)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (statusUpdate.Coordinate != null)
                {
                    this.CoordinateInformation = statusUpdate.Coordinate.Point.Position.Latitude.ToString() + ", " + statusUpdate.Coordinate.Point.Position.Longitude.ToString();
                }
                else
                {
                    this.CoordinateInformation = "Getting your location now, please be patient.";
                }
            });
        }
    }
}

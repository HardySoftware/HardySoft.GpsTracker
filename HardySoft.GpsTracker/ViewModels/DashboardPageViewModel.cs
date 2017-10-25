namespace HardySoft.GpsTracker.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using HardySoft.GpsTracker.BackgroundTasks;
    using HardySoft.GpsTracker.Models;
    using HardySoft.GpsTracker.Services.Gpx;
    using HardySoft.GpsTracker.Services.Location;
    using HardySoft.GpsTracker.Services.Models;
    using Prism.Commands;
    using Prism.Windows.AppModel;
    using Prism.Windows.Mvvm;
    using Prism.Windows.Navigation;
    using Windows.ApplicationModel.Background;
    using Windows.ApplicationModel.Core;
    using Windows.ApplicationModel.ExtendedExecution;
    using Windows.Devices.Geolocation;
    using Windows.Storage;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A view model for dashboard page.
    /// </summary>
    public class DashboardPageViewModel : ViewModelBase
    {
        /// <summary>
        /// The background task name.
        /// </summary>
        private const string BackgroundTaskName = "GPS_Tracking_Task";

        /// <summary>
        /// A location tracker implementation.
        /// </summary>
        private readonly ILocationTracker locationTracker;

        /// <summary>
        /// A GPX handler implementation.
        /// </summary>
        private readonly IGpxHandler gpxHandler;

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
        /// A timer used to refresh the information on screen periodically.
        /// </summary>
        private DispatcherTimer refreshTimer;

        /// <summary>
        /// A unique identifier of each individual tracking.
        /// </summary>
        private string trackingId;

        /// <summary>
        /// A flag to control the location changed event handler, make sure the previous operation is completed before it handles next one.
        /// </summary>
        private bool previousLocationEventChangedHandled = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardPageViewModel"/> class.
        /// </summary>
        /// <param name="gpxHandler">The Gpx handler implementation it depends on.</param>
        /// /// <param name="locationTracker">The location tracker implementation it depends on.</param>
        public DashboardPageViewModel(IGpxHandler gpxHandler, ILocationTracker locationTracker)
        {
            this.gpxHandler = gpxHandler ?? throw new ArgumentNullException(nameof(gpxHandler));
            this.locationTracker = locationTracker ?? throw new ArgumentNullException(nameof(locationTracker));

            this.status = TrackingStatus.Stopped;
            this.StartPauseClickedCommand = new DelegateCommand<ItemClickEventArgs>(this.OnStartPauseClicked, this.CanStartPauseClick);
            this.StopClickedCommand = new DelegateCommand<ItemClickEventArgs>(this.OnStopClicked, this.CanStopClick);
            this.SelectedActivity = ActivityTypes.Unknown;
            this.CoordinateInformation = "Your location information";

            this.refreshTimer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 30)
            };

            this.refreshTimer.Tick += async (object sender, object e) => { await this.DisplayMostRecentLocationData(string.Empty); };

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
        public ICommand StartPauseClickedCommand { get; private set; }

        /// <summary>
        /// Gets the command to handle stop button clicked event.
        /// </summary>
        public ICommand StopClickedCommand { get; private set; }

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

        /// <inheritdoc />
        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            // Check if the background task is active, the "IsBackgroundTaskActive" state is set by the background task.
            bool isBackgroundTaskActive =
                ApplicationData.Current.LocalSettings.Values.ContainsKey("IsBackgroundTaskActive") &&
                (bool)ApplicationData.Current.LocalSettings.Values["IsBackgroundTaskActive"];

            if (isBackgroundTaskActive)
            {
                this.refreshTimer.Start();
            }

            base.OnNavigatedTo(e, viewModelState);
        }

        /// <inheritdoc />
        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            Window.Current.VisibilityChanged -= new WindowVisibilityChangedEventHandler(this.VisibilityChanged);
            ApplicationData.Current.LocalSettings.Values["IsAppVisible"] = false;

            base.OnNavigatingFrom(e, viewModelState, suspending);
        }

        /// <summary>
        /// Handle start/pause button clicked event.
        /// </summary>
        /// <param name="argument">The event argument.</param>
        private async void OnStartPauseClicked(ItemClickEventArgs argument)
        {
            this.trackingId = DateTime.Now.ToString("yyyyMMddHHmmss");

            Window.Current.VisibilityChanged += new WindowVisibilityChangedEventHandler(this.VisibilityChanged);

            var locationAccessStatus = await Geolocator.RequestAccessAsync();

            if (locationAccessStatus == GeolocationAccessStatus.Allowed)
            {
                // Step 1, create background task as fall back plan for extended session. RequestAccessAsync must be called on the UI thread.
                var backGroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                if (backGroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed || backGroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
                {
                    this.StartBackgroundTask();
                    Debug.WriteLine($"{DateTime.Now} - Background task started");
                    this.refreshTimer.Start();
                    await this.DisplayMostRecentLocationData("In progress.");
                }

                // Step 2, request extended session.
                await this.StartExtendedExecution();
            }
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
        /// Handle stop button clicked event.
        /// </summary>
        /// <param name="argument">The event argument.</param>
        private async void OnStopClicked(ItemClickEventArgs argument)
        {
            this.locationTracker.OnTrackingProgressChangedEvent -= this.LocationTracker_OnTrackingProgressChangedEvent;
            this.locationTracker.StopTracking();

            this.FindAndCancelExistingBackgroundTask();

            await this.gpxHandler.ComposeGpxFile(this.trackingId, this.SelectedActivity.ToString());
        }

        /// <summary>
        /// Checks if the stop button can be clicked.
        /// </summary>
        /// <param name="argument">The argument event.</param>
        /// <returns>True if the click is allowed.</returns>
        private bool CanStopClick(ItemClickEventArgs argument)
        {
            // return this.SelectedActivity == null ? false : true;
            return true;
        }

        /// <summary>
        /// Start an extended execution session to track GPS activities.
        /// </summary>
        /// <returns>The asynchronous task.</returns>
        private async Task StartExtendedExecution()
        {
            var newSession = new ExtendedExecutionSession();
            newSession.Reason = ExtendedExecutionReason.LocationTracking;
            newSession.Revoked += this.SessionRevoked;

            var extendedSessionResult = await newSession.RequestExtensionAsync();

            switch (extendedSessionResult)
            {
                case ExtendedExecutionResult.Allowed:
                    // TODO define interval based on activity type.
                    await this.locationTracker.StartTracking(5, 100);
                    break;
                default:
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await this.DisplayMostRecentLocationData("Your decision makes it unable to track GPS locations frequently. You can only get one location reading every 15 minutes.");
                    });
                    break;
            }
        }

        /// <summary>
        /// Handles event when an extended execution session is revoked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event argument.</param>
        private async void SessionRevoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            switch (args.Reason)
            {
                case ExtendedExecutionRevokedReason.Resumed:
                    // The app returns to the foreground.
                    await this.StartExtendedExecution();
                    break;
            }
        }

        /// <summary>
        /// An event handler to handle GPS track changed event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="statusUpdate">The event argument with detailed data.</param>
        private async void LocationTracker_OnTrackingProgressChangedEvent(object sender, LocationResponseEventArgs statusUpdate)
        {
            if (this.previousLocationEventChangedHandled)
            {
                this.previousLocationEventChangedHandled = false;

                string message;
                if (statusUpdate.Coordinate != null)
                {
                    message = statusUpdate.Coordinate.Point.Position.Latitude.ToString() + ", " + statusUpdate.Coordinate.Point.Position.Longitude.ToString();
                    await this.gpxHandler.RecordLocationAsync(this.trackingId, statusUpdate.Coordinate);
                }
                else
                {
                    message = "Getting your location now, please be patient.";
                }

                await this.DisplayMostRecentLocationData(message);

                this.previousLocationEventChangedHandled = true;
            }
        }

        /// <summary>
        /// This is the event handler for VisibilityChanged events.
        /// </summary>
        /// <param name="sender">The event sender object.</param>
        /// <param name="e">Event data that can be examined for the current visibility state.</param>
        private void VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["IsAppVisible"] = e.Visible;

            if (e.Visible)
            {
                this.refreshTimer.Start();
            }
            else
            {
                this.refreshTimer.Stop();
            }
        }

        /// <summary>
        /// Starts the background task.
        /// </summary>
        private void StartBackgroundTask()
        {
            // Make sure only 1 task is running.
            this.FindAndCancelExistingBackgroundTask();

            // Register the background task.
            var backgroundTaskBuilder = new BackgroundTaskBuilder()
            {
                TaskEntryPoint = typeof(GpsTrackingTask).FullName,
                Name = BackgroundTaskName,
            };

            backgroundTaskBuilder.SetTrigger(new TimeTrigger(15, false));
            backgroundTaskBuilder.Register();
        }

        /// <summary>
        /// Finds a previously registered background task and cancels it (if present)
        /// </summary>
        private void FindAndCancelExistingBackgroundTask()
        {
            foreach (var backgroundTask in BackgroundTaskRegistration.AllTasks.Values)
            {
                if (backgroundTask.Name == BackgroundTaskName)
                {
                    ((BackgroundTaskRegistration)backgroundTask).Unregister(true);
                    break;
                }
            }
        }

        /// <summary>
        /// Gets most recent location data from background task and display on screen.
        /// </summary>
        /// <param name="message">The message to display instead of the location data.</param>
        /// <returns>The asynchronous task object.</returns>
        private async Task DisplayMostRecentLocationData(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    this.CoordinateInformation = message;
                }
                else
                {
                    // TODO implement the logic.
                    this.CoordinateInformation = string.Empty;
                }
            });
        }
    }
}
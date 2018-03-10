namespace HardySoft.GpsTracker.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using HardySoft.GpsTracker.BackgroundTasks;
    using HardySoft.GpsTracker.Models;
    using HardySoft.GpsTracker.Services.Gpx;
    using HardySoft.GpsTracker.Services.Gpx.Models;
    using HardySoft.GpsTracker.Services.LocalSetting;
    using HardySoft.GpsTracker.Services.Location;
    using Microsoft.HockeyApp;
    using Prism.Commands;
    using Prism.Windows.Mvvm;
    using Prism.Windows.Navigation;
    using Windows.ApplicationModel.Background;
    using Windows.ApplicationModel.Core;
    using Windows.ApplicationModel.ExtendedExecution;
    using Windows.Devices.Geolocation;
    using Windows.System.Threading;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A view model for dashboard page.
    /// </summary>
    public class TrackingPageViewModel : ViewModelBase
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
        /// A setting operator implementation.
        /// </summary>
        private readonly ISettingOperator settingOperator;

        /// <summary>
        /// The status of the tracking.
        /// </summary>
        private TrackingStatus status;

        /// <summary>
        /// The selected activity;
        /// </summary>
        private ActivityType selectedActivity;

        /// <summary>
        /// The text information of the coordinate from location tracking service.
        /// </summary>
        private string coordinateInformation;

        /// <summary>
        /// A timer used to refresh the information on screen periodically.
        /// </summary>
        private DispatcherTimer refreshTimer;

        /// <summary>
        /// A timer used to control the interval to fetch location information.
        /// </summary>
        private ThreadPoolTimer locationFechingTimer;

        /// <summary>
        /// A unique identifier of each individual tracking.
        /// </summary>
        private string trackingId;

        /// <summary>
        /// An indicator to tell the application what tracking mechanism to use.
        /// </summary>
        private TrackingMechanism trackingMechanism;

        /// <summary>
        /// The date time when most recent location update happens. The update could
        /// be from location changed event or fetching timer event..
        /// </summary>
        private DateTime mostRecentLocationUpdateTime;

        /// <summary>
        /// An indicator to tell if the location timer has triggered the location fetching operation.
        /// </summary>
        private bool isFetchingLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackingPageViewModel"/> class.
        /// </summary>
        /// <param name="gpxHandler">The Gpx handler implementation it depends on.</param>
        /// <param name="locationTracker">The location tracker implementation it depends on.</param>
        /// <param name="settingOperator">The setting operator implementation it depends on.</param>
        public TrackingPageViewModel(IGpxHandler gpxHandler, ILocationTracker locationTracker, ISettingOperator settingOperator)
        {
            this.gpxHandler = gpxHandler ?? throw new ArgumentNullException(nameof(gpxHandler));
            this.locationTracker = locationTracker ?? throw new ArgumentNullException(nameof(locationTracker));
            this.settingOperator = settingOperator ?? throw new ArgumentNullException(nameof(settingOperator));

            this.status = TrackingStatus.Stopped;
            this.StartPauseClickedCommand = new DelegateCommand<ItemClickEventArgs>(this.OnStartPauseClicked, this.CanStartPauseClick);
            this.StopClickedCommand = new DelegateCommand<ItemClickEventArgs>(this.OnStopClicked, this.CanStopClick);
            this.SelectedActivity = ActivityType.Unknown;
            this.CoordinateInformation = "Your location information";

            this.settingOperator.ResetSettings();

            this.refreshTimer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 30)
            };

            this.refreshTimer.Tick += async (object sender, object e) => { await this.DisplayMostRecentLocationData(string.Empty); };

            Debug.WriteLine($"{DateTime.Now} - Attached LocationTracker_OnTrackingProgressChangedEvent event handler.");
            this.locationTracker.OnTrackingProgressChangedEvent += this.LocationTracker_OnTrackingProgressChangedEvent;

            this.trackingMechanism = TrackingMechanism.LocationFetchingTimer;
        }

        /// <summary>
        /// Gets a list of activity types and their display texts.
        /// </summary>
        public ObservableCollection<ActivityTypeDisplay> SupportedActivityTypes => new ObservableCollection<ActivityTypeDisplay>(ActivityTypeDisplay.GetAllActivityTypes());

        /// <summary>
        /// Gets or sets the selected activity.
        /// </summary>
        public ActivityType SelectedActivity
        {
            get
            {
                return this.selectedActivity;
            }

            set
            {
                this.SetProperty(ref this.selectedActivity, value);

                // The enabled statuses of 2 buttons are also impacted by this value.
                this.OnPropertyChanged(nameof(this.IsStartPauseButtonEnabled));
                this.OnPropertyChanged(nameof(this.IsStopButtonEnabled));
            }
        }

        /// <summary>
        /// Gets a value indicating whether the activity option combo box is enabled or not.
        /// </summary>
        public bool IsActivityOptionEnabled
        {
            get
            {
                if (this.status == TrackingStatus.Started)
                {
                    return false;
                }

                return true;
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
        public string StartPauseButtonDescription => "Start tracking";

        /// <summary>
        /// Gets a value indicating whether the start/pause button is enabled or not.
        /// </summary>
        public bool IsStartPauseButtonEnabled
        {
            get
            {
                if (this.SelectedActivity != ActivityType.Unknown)
                {
                    if (this.status == TrackingStatus.Stopped)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the icon for the stop button.
        /// </summary>
        public Symbol StopButtonIcon => Symbol.Stop;

        /// <summary>
        /// Gets the description text for the start/pause button.
        /// </summary>
        public string StopButtonDescription => "Stop tracking";

        /// <summary>
        /// Gets a value indicating whether the stop button is enabled or not.
        /// </summary>
        public bool IsStopButtonEnabled
        {
            get
            {
                if (this.SelectedActivity != ActivityType.Unknown)
                {
                    if (this.status == TrackingStatus.Started)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

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
            bool isBackgroundTaskActive = this.settingOperator.GetBackgroundTaskActiveStatus();

            if (isBackgroundTaskActive)
            {
                this.refreshTimer.Start();
            }

            this.settingOperator.SetAppVisibilityStatus(true);
            base.OnNavigatedTo(e, viewModelState);
        }

        /// <inheritdoc />
        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            Window.Current.VisibilityChanged -= new WindowVisibilityChangedEventHandler(this.VisibilityChanged);
            this.settingOperator.SetAppVisibilityStatus(false);

            base.OnNavigatingFrom(e, viewModelState, suspending);
        }

        /// <summary>
        /// Handle start/pause button clicked event.
        /// </summary>
        /// <param name="argument">The event argument.</param>
        private async void OnStartPauseClicked(ItemClickEventArgs argument)
        {
            this.trackingId = DateTime.Now.ToString("yyyyMMddHHmmss");

            Window.Current.VisibilityChanged += this.VisibilityChanged;

            var locationAccessStatus = await Geolocator.RequestAccessAsync();

            if (locationAccessStatus == GeolocationAccessStatus.Allowed)
            {
                // Set the accuracy expectation in setting, so that background task can access the information.
                if (this.SelectedActivity == ActivityType.Unknown)
                {
                    this.settingOperator.SetGpsAccuracyExpectation(null);
                }
                else
                {
                    var activityDetail = this.SupportedActivityTypes.Where(x => x.ActivityType == this.SelectedActivity).First();
                    this.settingOperator.SetGpsAccuracyExpectation(activityDetail.DesiredAccuracy);
                }

                this.settingOperator.SetTrackingId(this.trackingId);
                this.mostRecentLocationUpdateTime = DateTime.Now;

                // Step 1, create background task as fall back plan for extended session. RequestAccessAsync must be called on the UI thread.
                var backGroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                if (backGroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed || backGroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
                {
                    this.StartBackgroundTask();
                    Debug.WriteLine($"{DateTime.Now} - Background task started");
                    this.refreshTimer.Start();
                    await this.DisplayMostRecentLocationData("In progress.");
                }

                this.status = TrackingStatus.Started;
                this.OnPropertyChanged(nameof(this.IsStartPauseButtonEnabled));
                this.OnPropertyChanged(nameof(this.IsStopButtonEnabled));
                this.OnPropertyChanged(nameof(this.IsActivityOptionEnabled));

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
            return true;
        }

        /// <summary>
        /// Handle stop button clicked event.
        /// </summary>
        /// <param name="argument">The event argument.</param>
        private async void OnStopClicked(ItemClickEventArgs argument)
        {
            this.settingOperator.ResetSettings();
            Window.Current.VisibilityChanged -= this.VisibilityChanged;

            this.StopLocationUpdateTracking();
            this.StopLocationIntervalTracking();
            this.FindAndCancelExistingBackgroundTask();

            int wayPointNumber = await this.gpxHandler.ComposeGpxFile(this.trackingId, this.SelectedActivity.ToString());

            this.status = TrackingStatus.Stopped;
            this.OnPropertyChanged(nameof(this.IsStartPauseButtonEnabled));
            this.OnPropertyChanged(nameof(this.IsStopButtonEnabled));
            this.OnPropertyChanged(nameof(this.IsActivityOptionEnabled));

            await this.DisplayMostRecentLocationData($"Gpx file created with {wayPointNumber} points collected.");

            this.refreshTimer.Stop();
        }

        /// <summary>
        /// Checks if the stop button can be clicked.
        /// </summary>
        /// <param name="argument">The argument event.</param>
        /// <returns>True if the click is allowed.</returns>
        private bool CanStopClick(ItemClickEventArgs argument)
        {
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
                    if (this.trackingMechanism == TrackingMechanism.LocationServiceProgressChangedEvent)
                    {
                        await this.StartLocationUpdateTracking();
                    }
                    else if (this.trackingMechanism == TrackingMechanism.LocationFetchingTimer)
                    {
                        this.StartLocationIntervalTracking();
                    }
                    else if (this.trackingMechanism == TrackingMechanism.Hybrid)
                    {
                        await this.StartLocationUpdateTracking();
                        this.StartLocationIntervalTracking();
                    }

                    this.refreshTimer.Start();

                    break;
                default:
                    await this.DisplayMostRecentLocationData("Your decision makes it unable to track GPS locations frequently. You can only get one location reading every 15 minutes.");
                    break;
            }
        }

        /// <summary>
        /// Starts the location tracking by using location service's progress changed event.
        /// </summary>
        /// <returns>The asynchronous task.</returns>
        private async Task StartLocationUpdateTracking()
        {
            var activityDetail = this.SupportedActivityTypes.First(x => x.ActivityType == this.SelectedActivity);
            await this.locationTracker.StartTracking(activityDetail.DesiredAccuracy, activityDetail.TrackingInterval);
        }

        /// <summary>
        /// An event handler to handle GPS track changed event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="statusUpdate">The event argument with detailed data.</param>
        private async void LocationTracker_OnTrackingProgressChangedEvent(object sender, LocationResponseEventArgs statusUpdate)
        {
            // This mechanism is purely managed by OS, there are lots of time the app could not get any update for long time.
            var eventProperties = new Dictionary<string, string>()
                {
                    { "Coordinate null", (statusUpdate.Coordinate == null).ToString() },
                    { "Tracking id", this.trackingId }
                };

            HockeyClient.Current.TrackEvent("Location changed", eventProperties, null);
            await this.DisplayMostRecentLocationData(statusUpdate.Coordinate);
            await this.gpxHandler.RecordLocationAsync(this.trackingId, statusUpdate.Coordinate, "source E");
            this.mostRecentLocationUpdateTime = DateTime.Now;
        }

        /// <summary>
        /// Stops location tracking by removing location service's progress changed event.
        /// </summary>
        private void StopLocationUpdateTracking()
        {
            this.locationTracker.OnTrackingProgressChangedEvent -= this.LocationTracker_OnTrackingProgressChangedEvent;
            this.locationTracker.StopTracking();
        }

        /// <summary>
        /// Starts the location tracking by using system timer.
        /// </summary>
        private void StartLocationIntervalTracking()
        {
            var activityDetail = this.SupportedActivityTypes.First(x => x.ActivityType == this.SelectedActivity);
            this.isFetchingLocation = false;

            var handler = new TimerElapsedHandler(this.LocationFechingTimer_Tick);

            if (this.trackingMechanism == TrackingMechanism.LocationFetchingTimer)
            {
                this.locationFechingTimer = ThreadPoolTimer.CreatePeriodicTimer(handler, TimeSpan.FromSeconds(activityDetail.TrackingInterval));
            }
            else if (this.trackingMechanism == TrackingMechanism.Hybrid)
            {
                this.locationFechingTimer = ThreadPoolTimer.CreatePeriodicTimer(handler, TimeSpan.FromSeconds(activityDetail.TrackingInterval / 2));
            }

            // Don't wait for timer to tick.
            this.LocationFechingTimer_Tick(null);
        }

        /// <summary>
        /// An event handler to handle location fetching timer ticks.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        private async void LocationFechingTimer_Tick(ThreadPoolTimer source)
        {
            if (this.isFetchingLocation)
            {
                // prevent timer to run if the operation has not finished yet.
                return;
            }

            try
            {
                this.isFetchingLocation = true;
                await this.gpxHandler.RecordCommentAsync(this.trackingId, $"Fetching timer ticked at {DateTime.Now.ToUniversalTime().ToString("o")}.");

                var activityDetail = this.SupportedActivityTypes.Where(x => x.ActivityType == this.SelectedActivity).First();

                if (this.trackingMechanism == TrackingMechanism.Hybrid)
                {
                    if (this.mostRecentLocationUpdateTime.AddSeconds(activityDetail.TrackingInterval) > DateTime.Now)
                    {
                        // If the location changed event provides frequent update, we don't need fetching timer to compensate scarce location tracking.
                        await this.gpxHandler.RecordCommentAsync(this.trackingId, $"Hybrid mode, skip running because it's too close to previous run {this.mostRecentLocationUpdateTime.ToUniversalTime().ToString("o")}.");
                        return;
                    }
                }

                Debug.WriteLine($"{DateTime.Now} - Fetching location from timer.");
                var sw = new Stopwatch();
                sw.Start();

                // This step seems to be very slow, maybe it tends to consume more battery.
                var currentLocation = await this.locationTracker.GetCurrentLocation(activityDetail.DesiredAccuracy);

                sw.Stop();

                await this.DisplayMostRecentLocationData(currentLocation);
                await this.gpxHandler.RecordLocationAsync(this.trackingId, currentLocation, $"source T({sw.ElapsedMilliseconds} ms)");
                this.mostRecentLocationUpdateTime = DateTime.Now;
            }
            finally
            {
                this.isFetchingLocation = false;
            }
        }

        /// <summary>
        /// Stops the location tracking by using system timer.
        /// </summary>
        private void StopLocationIntervalTracking()
        {
            if (this.locationFechingTimer != null)
            {
                this.locationFechingTimer.Cancel();
            }
        }

        /// <summary>
        /// Handles event when an extended execution session is revoked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event argument.</param>
        private async void SessionRevoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            await this.gpxHandler.RecordCommentAsync(this.trackingId, "Extended session revoked");

            switch (args.Reason)
            {
                case ExtendedExecutionRevokedReason.Resumed:
                    // The app returns to the foreground.
                    await this.StartExtendedExecution();
                    break;
            }
        }

        /// <summary>
        /// This is the event handler for VisibilityChanged events.
        /// </summary>
        /// <param name="sender">The event sender object.</param>
        /// <param name="e">Event data that can be examined for the current visibility state.</param>
        private void VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            this.settingOperator.SetAppVisibilityStatus(e.Visible);

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
            });
        }

        /// <summary>
        /// Gets most recent location data from background task and display on screen.
        /// </summary>
        /// <param name="currentLocation">The location with current coordinate.</param>
        /// <returns>The asynchronous task object.</returns>
        private async Task DisplayMostRecentLocationData(Geocoordinate currentLocation)
        {
            string message = string.Empty;

            if (currentLocation != null)
            {
                Debug.WriteLine($"{DateTime.Now} - GPS position or status has changed in tracking view model.");
                message = currentLocation.Point.Position.Latitude.ToString() + ", " + currentLocation.Point.Position.Longitude.ToString();
            }
            else
            {
                message = "Getting your location now, please be patient.";
            }

            await this.DisplayMostRecentLocationData(message);
        }
    }
}
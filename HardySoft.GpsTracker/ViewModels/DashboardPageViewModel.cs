namespace HardySoft.GpsTracker.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using HardySoft.GpsTracker.Models;
    using HardySoft.GpsTracker.Services.Gpx;
    using HardySoft.GpsTracker.Services.Models;
    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Windows.Mvvm;
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
        /// The status of the tracking.
        /// </summary>
        private TrackingStatus status;

        /// <summary>
        /// The selected activity;
        /// </summary>
        private ActivityTypes selectedActivity;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardPageViewModel"/> class.
        /// </summary>
        /// <param name="gpxHandler">The Gpx handler implementation it depends on.</param>
        public DashboardPageViewModel(IGpxHandler gpxHandler)
        {
            this.gpxHandler = gpxHandler ?? throw new ArgumentNullException(nameof(gpxHandler));

            this.status = TrackingStatus.Stopped;
            this.StartPauseClickedCommand = new DelegateCommand<ItemClickEventArgs>(this.OnStartPauseClicked, this.CanStartPauseClicked);
            this.SelectedActivity = ActivityTypes.Unknown;
        }

        /// <summary>
        /// Gets a list of activity types and their display texts.
        /// </summary>
        public ObservableCollection<ActivityTypeDisplay> SupportedActivityTypeDisplay => new ObservableCollection<ActivityTypeDisplay>(ActivityTypeDisplay.GetAllActivityTypeDisplay());

        /// <summary>
        /// Gets or sets the selected activity.
        /// </summary>
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
        /// Handle start/pause button clicked event.
        /// </summary>
        /// <param name="argument">The event argument.</param>
        private void OnStartPauseClicked(ItemClickEventArgs argument)
        {
        }

        /// <summary>
        /// Checks if the start/pause button can be clicked.
        /// </summary>
        /// <param name="argument">The argument event.</param>
        /// <returns>True if the click is allowed.</returns>
        private bool CanStartPauseClicked(ItemClickEventArgs argument)
        {
            // return this.SelectedActivity == null ? false : true;
            return true;
        }
    }
}

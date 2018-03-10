namespace HardySoft.GpsTracker.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using HardySoft.GpsTracker.Models;
    using HardySoft.GpsTracker.Services.Gpx;
    using HardySoft.GpsTracker.Services.LocalSetting;
    using Prism.Commands;
    using Prism.Windows.AppModel;
    using Prism.Windows.Mvvm;
    using Prism.Windows.Navigation;
    using Windows.ApplicationModel.Core;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A view model class for setting page.
    /// </summary>
    public class SettingPageViewModel : ViewModelBase
    {
        /// <summary>
        /// The session state service.
        /// </summary>
        private readonly ISessionStateService sessionService;

        /// <summary>
        /// A GPX handler implementation.
        /// </summary>
        private readonly IGpxHandler gpxHandler;

        /// <summary>
        /// A setting operator implementation.
        /// </summary>
        private readonly ISettingOperator settingOperator;

        /// <summary>
        /// The information text for this view.
        /// </summary>
        private string informationText;

        /// <summary>
        /// The indicator to determine if the temp file delete button is enabled or not.
        /// </summary>
        private bool isTempFileDeleteButtonEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingPageViewModel"/> class.
        /// </summary>
        /// <param name="sessionService">The session state service it depends on.</param>
        /// <param name="gpxHandler">The Gpx handler implementation it depends on.</param>
        /// <param name="settingOperator">The setting operator implementation it depends on.</param>
        public SettingPageViewModel(ISessionStateService sessionService, IGpxHandler gpxHandler, ISettingOperator settingOperator)
        {
            this.gpxHandler = gpxHandler ?? throw new ArgumentNullException(nameof(gpxHandler));
            this.settingOperator = settingOperator ?? throw new ArgumentNullException(nameof(settingOperator));
            this.sessionService = sessionService;

            this.ClearTempFileButtonClickedCommand = new DelegateCommand<ItemClickEventArgs>(this.OnClearTempFileButtonClicked, this.CanClearTempFileButtonClick);
            this.isTempFileDeleteButtonEnabled = true;
        }

        /// <summary>
        /// Gets or sets the information text of this view.
        /// </summary>
        public string InformationText
        {
            get
            {
                return this.informationText;
            }

            set
            {
                this.SetProperty(ref this.informationText, value);
            }
        }

        /// <summary>
        /// Gets a list of activity types and their display texts.
        /// </summary>
        public ObservableCollection<TrackingMechanismDisplay> SupportedTrackingMechanisms => new ObservableCollection<TrackingMechanismDisplay>(TrackingMechanismDisplay.GetAllTrackingMechanisms());

        /// <summary>
        /// Gets or sets the selected tracking mechanism.
        /// </summary>
        public TrackingMechanism SelectedTrackingMechanism
        {
            get
            {
                return this.GetSavedTrackingMechanism();
            }

            set
            {
                this.settingOperator.SetTrackingMechanism((int)value);
            }
        }

        /// <summary>
        /// Gets the command to button clicked event.
        /// </summary>
        public ICommand ClearTempFileButtonClickedCommand { get; private set; }

        /// <summary>
        /// Called when navigation is performed to a page.
        /// </summary>
        /// <param name="e">The Prism.Windows.Navigation.NavigatedToEventArgs instance containing the event data.</param>
        /// <param name="viewModelState">The state of the view model.</param>
        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
        }

        /// <summary>
        /// Handle button clicked event.
        /// </summary>
        /// <param name="argument">The event argument.</param>
        private async void OnClearTempFileButtonClicked(ItemClickEventArgs argument)
        {
            try
            {
                this.isTempFileDeleteButtonEnabled = false;
                await this.DisplayInformation($"Deleting temporary files...");
                var deleteResult = await this.gpxHandler.ClearTemporaryGpxWaypointFiles();
                await this.DisplayInformation($"Removed {deleteResult.Key} files, freed {deleteResult.Value / 1024} KB space.");
            }
            finally
            {
                this.isTempFileDeleteButtonEnabled = true;
            }
        }

        /// <summary>
        /// Checks if the button can be clicked.
        /// </summary>
        /// <param name="argument">The argument event.</param>
        /// <returns>True if the click is allowed.</returns>
        private bool CanClearTempFileButtonClick(ItemClickEventArgs argument)
        {
            return this.isTempFileDeleteButtonEnabled;
        }

        /// <summary>
        /// Gets most recent location data from background task and display on screen.
        /// </summary>
        /// <param name="message">The message to display instead of the location data.</param>
        /// <returns>The asynchronous task object.</returns>
        private async Task DisplayInformation(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    this.InformationText = message;
                }
            });
        }

        private TrackingMechanism GetSavedTrackingMechanism()
        {
            var trackingMechanismId = this.settingOperator.GetTrackingMechanismId();

            if (trackingMechanismId == null)
            {
                return TrackingMechanism.LocationServiceProgressChangedEvent;
            }
            else
            {
                return (TrackingMechanism)trackingMechanismId.Value;
            }
        }
    }
}

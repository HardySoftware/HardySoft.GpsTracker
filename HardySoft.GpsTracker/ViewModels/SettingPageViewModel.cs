namespace HardySoft.GpsTracker.ViewModels
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using Prism.Commands;
    using Prism.Windows.AppModel;
    using Prism.Windows.Mvvm;
    using Prism.Windows.Navigation;
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
        /// Text 1.
        /// </summary>
        private string text1;

        /// <summary>
        /// Text 2.
        /// </summary>
        private string text2;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingPageViewModel"/> class.
        /// </summary>
        /// <param name="sessionService">The session state service it depends on.</param>
        public SettingPageViewModel(ISessionStateService sessionService)
        {
            this.ButtonClickedCommand = new DelegateCommand<ItemClickEventArgs>(this.OnButtonClicked, this.CanButtonClick);
            this.sessionService = sessionService;
        }

        /// <summary>
        /// Gets or sets text 1.
        /// </summary>
        [RestorableState]
        public string Text1
        {
            get
            {
                return this.text1;
            }

            set
            {
                this.SetProperty(ref this.text1, value);
            }
        }

        /// <summary>
        /// Gets or sets text 1.
        /// </summary>
        [RestorableState]
        public string Text2
        {
            get
            {
                return this.text2;
            }

            set
            {
                this.SetProperty(ref this.text2, value);
            }
        }

        /// <summary>
        /// Gets the command to button clicked event.
        /// </summary>
        public ICommand ButtonClickedCommand { get; private set; }

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
        private void OnButtonClicked(ItemClickEventArgs argument)
        {
            this.Text1 = "The value is set by button click";
        }

        /// <summary>
        /// Checks if the button can be clicked.
        /// </summary>
        /// <param name="argument">The argument event.</param>
        /// <returns>True if the click is allowed.</returns>
        private bool CanButtonClick(ItemClickEventArgs argument)
        {
            return true;
        }
    }
}

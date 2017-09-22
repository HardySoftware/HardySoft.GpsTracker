namespace HardySoft.GpsTracker
{
    using HardySoft.GpsTracker.Models;
    using HardySoft.GpsTracker.ViewModels;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            this.DataContext = new MainPageViewModel();
        }

        /// <summary>
        /// An event handler to handle hamburger menu clicked event.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">The event argument.</param>
        private void OnMenuItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as MenuItem;

            // this.contentFrame.Navigate(menuItem.PageType);
            this.contentFrame.Navigate(menuItem.PageType);
        }
    }
}

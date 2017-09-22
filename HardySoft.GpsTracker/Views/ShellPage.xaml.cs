namespace HardySoft.GpsTracker.Views
{
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellPage"/> class.
        /// </summary>
        public ShellPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Set the content frame to the menu control content.
        /// </summary>
        /// <param name="frame">The frame to set to menu's content.</param>
        public void SetContentFrame(Frame frame)
        {
            this.hamburgerMenuControl.Content = frame;
        }
    }
}

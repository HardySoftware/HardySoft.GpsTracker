namespace HardySoft.GpsTracker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using HardySoft.GpsTracker.Models;
    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Navigation;

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

            this.hamburgerMenuControl.ItemsSource = MenuItem.GetMainItems();
            this.hamburgerMenuControl.OptionsItemsSource = MenuItem.GetOptionsItems();
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
            this.contentFrameInsideMenu.Navigate(menuItem.PageType);
        }
    }
}

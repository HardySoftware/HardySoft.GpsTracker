namespace HardySoft.GpsTracker.Models
{
    using System;
    using System.Collections.Generic;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A class for hamburger menu items.
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItem"/> class to use symbol text for icon of the menu item.
        /// </summary>
        /// <param name="icon">The symbol character for the icon.</param>
        /// <param name="name">The name of the page.</param>
        /// <param name="pageType">The type of the page.</param>
        public MenuItem(Symbol icon, string name, Type pageType)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.Icon = icon;
            this.Name = name;
            this.PageType = pageType ?? throw new ArgumentNullException(nameof(pageType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItem"/> class to use image for icon of the menu item.
        /// </summary>
        /// <param name="iconUri">The URI of the image for the icon.</param>
        /// <param name="name">The name of the page.</param>
        /// <param name="pageType">The type of the page.</param>
        public MenuItem(Uri iconUri, string name, Type pageType)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.ImageIconUri = iconUri ?? throw new ArgumentNullException(nameof(iconUri));
            this.Name = name;
            this.PageType = pageType ?? throw new ArgumentNullException(nameof(pageType));
        }

        /// <summary>
        /// Gets the icon of one menu item.
        /// </summary>
        public Symbol Icon { get; private set; }

        /// <summary>
        /// Gets the image icon URI of the menu item.
        /// </summary>
        public Uri ImageIconUri { get; private set; }

        /// <summary>
        /// Gets the name of one menu item.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of the page to go when the menu is clicked.
        /// </summary>
        public Type PageType { get; private set; }

        /// <summary>
        /// Get main menu items.
        /// </summary>
        /// <returns>A list of main menu items.</returns>
        public static List<MenuItem> GetMainItems()
        {
            var items = new List<MenuItem>();

            items.Add(new MenuItem(new Uri("ms-appx:///Assets/dotted-line-path.png"), "Start tracking", typeof(Views.DashboardPage)));
            items.Add(new MenuItem(Symbol.Map, "View my current location", typeof(Views.DashboardPage)));
            items.Add(new MenuItem(Symbol.BrowsePhotos, "View tracking history", typeof(Views.BlankPage)));

            return items;
        }

        /// <summary>
        /// Get option menu items.
        /// </summary>
        /// <returns>A list of option menu items.</returns>
        public static List<MenuItem> GetOptionsItems()
        {
            var items = new List<MenuItem>();
            items.Add(new MenuItem(Symbol.Setting, "Settings", typeof(Views.SettingPage)));
            return items;
        }
    }
}

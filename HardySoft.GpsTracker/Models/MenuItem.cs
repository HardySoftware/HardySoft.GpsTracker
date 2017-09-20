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
        /// Gets or sets the icon of one menu item.
        /// </summary>
        public Symbol Icon { get; set; }

        /// <summary>
        /// Gets or sets the name of one menu item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the page to go when the menu is clicked.
        /// </summary>
        public Type PageType { get; set; }

        /// <summary>
        /// Get main menu items.
        /// </summary>
        /// <returns>A list of main menu items.</returns>
        public static List<MenuItem> GetMainItems()
        {
            var items = new List<MenuItem>();

            items.Add(new MenuItem() { Icon = Symbol.Map, Name = "Start tracking", PageType = typeof(Views.BlankPage) });
            items.Add(new MenuItem() { Icon = Symbol.BrowsePhotos, Name = "View tracking history", PageType = typeof(Views.BlankPage) });
            return items;
        }

        /// <summary>
        /// Get option menu items.
        /// </summary>
        /// <returns>A list of option menu items.</returns>
        public static List<MenuItem> GetOptionsItems()
        {
            var items = new List<MenuItem>();
            items.Add(new MenuItem() { Icon = Symbol.Setting, Name = "Settings", PageType = typeof(Views.BlankPage) });
            return items;
        }
    }
}

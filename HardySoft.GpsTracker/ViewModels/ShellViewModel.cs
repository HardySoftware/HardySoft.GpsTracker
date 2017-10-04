﻿namespace HardySoft.GpsTracker.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using HardySoft.GpsTracker.Models;
    using HardySoft.GpsTracker.Support.Extensions;
    using Prism.Commands;
    using Prism.Windows.Navigation;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A view model class for the shell.
    /// </summary>
    public class ShellViewModel
    {
        /// <summary>
        /// A Prism navigation service to provide functions to navigate from one view to another.
        /// </summary>
        private readonly INavigationService navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        /// <param name="navigationService">The Prism navigation service the class depends on.</param>
        public ShellViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            this.NavigateCommand = new DelegateCommand<ItemClickEventArgs>(this.OnNavigate, this.CanNavigate);

            this.MainMenuItems = new ObservableCollection<MenuItem>(MenuItem.GetMainItems());
            this.OptionMenuItems = new ObservableCollection<MenuItem>(MenuItem.GetOptionsItems());
        }

        /// <summary>
        /// Gets the menu items used for top part of hamburger menu.
        /// </summary>
        public ObservableCollection<MenuItem> MainMenuItems { get; private set; }

        /// <summary>
        /// Gets the menu items for the bottom part of the hamburger menu.
        /// </summary>
        public ObservableCollection<MenuItem> OptionMenuItems { get; private set; }

        /// <summary>
        /// Gets the navigate command to other page.
        /// </summary>
        public ICommand NavigateCommand { get; private set; }

        /// <summary>
        /// Navigates to another page.
        /// </summary>
        /// <param name="argument">The page to navigate to.</param>
        private void OnNavigate(ItemClickEventArgs argument)
        {
            var menuItem = argument.ClickedItem as MenuItem;
            this.navigationService.Navigate(menuItem.PageType.GetPageToken(), null);
            return;
        }

        /// <summary>
        /// Checks if the designated page can be navigated to.
        /// </summary>
        /// <param name="argument">The page to navigate to.</param>
        /// <returns>True if the navigation is allowed.</returns>
        private bool CanNavigate(ItemClickEventArgs argument)
        {
            return true;
        }
    }
}

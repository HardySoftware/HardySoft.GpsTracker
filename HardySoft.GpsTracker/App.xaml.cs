namespace HardySoft.GpsTracker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using HardySoft.GpsTracker.Extensions;
    using HardySoft.GpsTracker.ViewModels;
    using HardySoft.GpsTracker.Views;
    using Microsoft.Practices.Unity;
    using Prism.Unity.Windows;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
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
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : PrismUnityApplication
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <remarks>
        /// This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </remarks>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Create a Shell for the MVVM application.
        /// </summary>
        /// <param name="rootFrame">The root frame object.</param>
        /// <returns>The shell.</returns>
        protected override UIElement CreateShell(Frame rootFrame)
        {
            var shell = this.Container.Resolve<ShellPage>();
            shell.DataContext = this.Container.Resolve<ShellViewModel>();
            shell.SetContentFrame(rootFrame);
            return shell;
        }

        /// <summary>
        /// When the application is launched.
        /// </summary>
        /// <param name="args">Argument object when window is launched/activated.</param>
        /// <returns>The completed task.</returns>
        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            this.NavigationService.Navigate(typeof(DashboardPage).GetPageToken(), null);
            return Task.CompletedTask;
        }
    }
}

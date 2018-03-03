namespace HardySoft.GpsTracker
{
    using System.Threading.Tasks;
    using HardySoft.GpsTracker.Services.Gpx;
    using HardySoft.GpsTracker.Services.LocalSetting;
    using HardySoft.GpsTracker.Services.Location;
    using HardySoft.GpsTracker.Support.Extensions;
    using HardySoft.GpsTracker.ViewModels;
    using HardySoft.GpsTracker.Views;
    using Microsoft.HockeyApp;
    using Microsoft.Practices.Unity;
    using Prism.Mvvm;
    using Prism.Unity.Windows;
    using Prism.Windows.AppModel;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

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
            var telemetryConfiguration = new TelemetryConfiguration()
            {
                Collectors = WindowsCollectors.Metadata | WindowsCollectors.Session | WindowsCollectors.UnhandledException
            };

            HockeyClient.Current.Configure("b43a48c928604536967ce7f9bb36a637", telemetryConfiguration);

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
            this.Container.RegisterType<IGpxHandler, GpxHandler>();
            this.Container.RegisterType<ILocationTracker, GpsLocationTracker>(new TransientLifetimeManager());
            this.Container.RegisterType<ISettingOperator, SettingOperator>(new ContainerControlledLifetimeManager());
            this.Container.RegisterInstance<ISessionStateService>(this.SessionStateService);

            // navigate to the default page of the application on start-up.
            this.NavigationService.Navigate(typeof(TrackingPage).GetPageToken(), null);

            // Set a factory for the ViewModelLocator to use the container to construct view models so their
            // dependencies get injected by the container
            ViewModelLocationProvider.SetDefaultViewModelFactory((viewModelType) => this.Container.Resolve(viewModelType));

            return Task.CompletedTask;
        }
    }
}

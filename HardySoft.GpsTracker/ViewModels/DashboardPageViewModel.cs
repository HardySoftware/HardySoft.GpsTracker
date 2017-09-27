namespace HardySoft.GpsTracker.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using HardySoft.GpsTracker.Models;
    using HardySoft.GpsTracker.Services.Gpx;
    using Prism.Mvvm;

    /// <summary>
    /// A view model for dashboard page.
    /// </summary>
    public class DashboardPageViewModel : BindableBase
    {
        private readonly IGpxHandler gpxHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardPageViewModel"/> class.
        /// </summary>
        /// <param name="gpxHandler">The Gpx handler implementation it depends on.</param>
        public DashboardPageViewModel(IGpxHandler gpxHandler)
        {
            this.gpxHandler = gpxHandler ?? throw new ArgumentNullException(nameof(gpxHandler));
        }

        /// <summary>
        /// Gets a list of supported activity types.
        /// </summary>
        public IEnumerable<ActivityTypes> SupportedActivityTypes => Enum.GetValues(typeof(ActivityTypes)).Cast<ActivityTypes>();
    }
}

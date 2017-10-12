namespace HardySoft.GpsTracker.BackgroundTasks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.ApplicationModel.Background;

    /// <summary>
    /// A background task to perform GPS tracking.
    /// </summary>
    public sealed class GpsTrackingTask : IBackgroundTask
    {
        /// <inheritdoc />
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine($"{DateTime.Now} - From Gps background task.");
        }
    }
}

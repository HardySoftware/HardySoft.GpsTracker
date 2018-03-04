namespace HardySoft.GpsTracker.BackgroundTasks
{
    using System;
    using System.Diagnostics;
    using HardySoft.GpsTracker.Services.Gpx;
    using HardySoft.GpsTracker.Services.LocalSetting;
    using HardySoft.GpsTracker.Services.Location;
    using Microsoft.HockeyApp;
    using Windows.ApplicationModel.Background;

    /// <summary>
    /// A background task to perform GPS tracking.
    /// </summary>
    public sealed class GpsTrackingTask : IBackgroundTask
    {
        /// <summary>
        /// The implementation of <see cref="ISettingOperator"/>
        /// </summary>
        private readonly ISettingOperator setting;

        /// <summary>
        /// The implementation of <see cref="IGpxHandler"/>.
        /// </summary>
        private readonly IGpxHandler gpxHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpsTrackingTask"/> class.
        /// </summary>
        public GpsTrackingTask()
        {
            this.setting = new SettingOperator();
            this.gpxHandler = new GpxHandler();
        }

        /// <inheritdoc />
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            Debug.WriteLine($"{DateTime.Now} - From Gps background task.");
            HockeyClient.Current.TrackTrace("Starting background task.");

            var accuracy = this.setting.GetGpsAccuracyExpectation();

            if (accuracy.HasValue)
            {
                var sw = new Stopwatch();
                sw.Start();

                var locationTracker = new GpsLocationTracker();
                var coordinate = await locationTracker.GetCurrentLocation(accuracy.Value);

                sw.Stop();
                await this.gpxHandler.RecordLocationAsync(this.setting.GetTrackingId(), coordinate, $"Source B({sw.ElapsedMilliseconds} ms)");
            }
            else
            {
                HockeyClient.Current.TrackTrace("Try to start background task without providing activity type.");
            }

            deferral.Complete();
        }
    }
}

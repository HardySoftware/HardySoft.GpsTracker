namespace HardySoft.GpsTracker.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HardySoft.GpsTracker.Support.Extensions;

    /// <summary>
    /// A wrapper class of <see cref="ActivityType"/> to include additional details required for the app.
    /// </summary>
    public class ActivityTypeDisplay
    {
        /// <summary>
        /// Gets the activity type.
        /// </summary>
        public ActivityType ActivityType { get; private set; }

        /// <summary>
        /// Gets the display value of the activity type.
        /// </summary>
        public string DisplayValue { get; private set; }

        /// <summary>
        /// Gets the interval in seconds for tracking purpose.
        /// </summary>
        public uint TrackingInterval { get; private set; }

        /// <summary>
        /// Gets the desired accuracy in meter for tracking purpose.
        /// </summary>
        public uint DesiredAccuracy { get; private set; }

        /// <summary>
        /// Gets a list of display values for all activity types.
        /// </summary>
        /// <returns>A collection of objects to wrap activity type and its display value.</returns>
        public static IEnumerable<ActivityTypeDisplay> GetAllActivityTypes()
        {
            var values = Enum.GetValues(typeof(ActivityType)).Cast<ActivityType>();

            var activityTypeDisplays = new List<ActivityTypeDisplay>();

            foreach (var value in values)
            {
                var description = value.GetDescription();

                uint interval, accuracy;

                GetActivityTrackingIntervalAndAccuracy(value, out interval, out accuracy);

                var detail = new ActivityTypeDisplay() { ActivityType = value, DisplayValue = description, TrackingInterval = interval, DesiredAccuracy = accuracy };

                activityTypeDisplays.Add(detail);
            }

            return activityTypeDisplays;
        }

        /// <summary>
        /// Gets the activity's desired tracing interval and accuracy.
        /// </summary>
        /// <param name="type">The activity type.</param>
        /// <param name="interval">Tracking interval in second.</param>
        /// <param name="accuracy">Tracking accuracy in meter.</param>
        private static void GetActivityTrackingIntervalAndAccuracy(ActivityType type, out uint interval, out uint accuracy)
        {
            switch (type)
            {
                case ActivityType.Hiking:
                    interval = 60;
                    accuracy = 20;
                    break;
                case ActivityType.Cycling:
                    interval = 30;
                    accuracy = 5;
                    break;
                default:
                    interval = 300;
                    accuracy = 50;
                    break;
            }
        }
    }
}

namespace HardySoft.GpsTracker.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HardySoft.GpsTracker.Support.Extensions;

    /// <summary>
    /// A wrapper class of <see cref="ActivityType"/> to include display value for UI.
    /// </summary>
    public class ActivityTypeDisplay
    {
        /// <summary>
        /// Gets or sets the activity type.
        /// </summary>
        public ActivityTypes ActivityType { get; set; }

        /// <summary>
        /// Gets or sets the display value of the activity type.
        /// </summary>
        public string DisplayValue { get; set; }

        /// <summary>
        /// Gets a list of display values for all activity types.
        /// </summary>
        /// <returns>A collection of objects to wrap activity type and its display value.</returns>
        public static IEnumerable<ActivityTypeDisplay> GetAllActivityTypeDisplay()
        {
            var values = Enum.GetValues(typeof(ActivityTypes)).Cast<ActivityTypes>();

            var activityTypeDisplays = new List<ActivityTypeDisplay>();

            foreach (var value in values)
            {
                var description = value.GetDescription();

                activityTypeDisplays.Add(new ActivityTypeDisplay() { ActivityType = value, DisplayValue = description });
            }

            return activityTypeDisplays;
        }
    }
}

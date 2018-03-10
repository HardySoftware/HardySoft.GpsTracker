namespace HardySoft.GpsTracker.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HardySoft.GpsTracker.Support.Extensions;

    /// <summary>
    /// A wrapper class of <see cref="TrackingMechanism"/> to include additional details required for the app.
    /// </summary>
    public class TrackingMechanismDisplay
    {
        /// <summary>
        /// Gets the tracking mechanism value.
        /// </summary>
        public TrackingMechanism TrackingMechanism { get; private set; }

        /// <summary>
        /// Gets the tracking mechanism  display value.
        /// </summary>
        public string DisplayValue { get; private set; }

        /// <summary>
        /// Gets a list of tracking mechanism details.
        /// </summary>
        /// <returns>A collection of objects to wrap tracking mechanism and its display value.</returns>
        public static IEnumerable<TrackingMechanismDisplay> GetAllTrackingMechanisms()
        {
            var values = Enum.GetValues(typeof(TrackingMechanism)).Cast<TrackingMechanism>();

            var activityTypeDisplays = new List<TrackingMechanismDisplay>();

            foreach (var value in values)
            {
                var description = value.GetDescription();

                var detail = new TrackingMechanismDisplay() { TrackingMechanism = value, DisplayValue = description };

                activityTypeDisplays.Add(detail);
            }

            return activityTypeDisplays;
        }
    }
}

namespace HardySoft.GpsTracker.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HardySoft.GpsTracker.Services.LocalSetting;
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

        /// <summary>
        /// Gets the saved tracking mechanism, if not found use the <see cref="TrackingMechanism.LocationServiceProgressChangedEvent"/> as default value.
        /// </summary>
        /// <param name="settingOperator">A setting operator implementation.</param>
        /// <returns>The saved tracking mechanism.</returns>
        public static TrackingMechanism GetSavedTrackingMechanism(ISettingOperator settingOperator)
        {
            var trackingMechanismId = settingOperator.GetTrackingMechanismId();

            if (trackingMechanismId == null)
            {
                return TrackingMechanism.LocationServiceProgressChangedEvent;
            }
            else
            {
                return (TrackingMechanism)trackingMechanismId.Value;
            }
        }
    }
}

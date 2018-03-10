namespace HardySoft.GpsTracker.Services.LocalSetting
{
    using System;
    using Windows.Storage;

    /// <summary>
    /// A class to implement the interface <c>ISettingOperator</c>.
    /// </summary>
    public class SettingOperator : ISettingOperator
    {
        /// <inheritdoc />
        public void ResetSettings()
        {
            this.SetBackgroundTaskActiveStatus(false);
            this.SetAppVisibilityStatus(false);
            this.SetGpsAccuracyExpectation(null);
            this.SetTrackingId(string.Empty);
        }

        /// <inheritdoc />
        public bool GetBackgroundTaskActiveStatus()
        {
            bool isBackgroundTaskActive =
                ApplicationData.Current.LocalSettings.Values.ContainsKey("IsBackgroundTaskActive") &&
                (bool)ApplicationData.Current.LocalSettings.Values["IsBackgroundTaskActive"];

            return isBackgroundTaskActive;
        }

        /// <inheritdoc />
        public void SetBackgroundTaskActiveStatus(bool isActive)
        {
            ApplicationData.Current.LocalSettings.Values["IsBackgroundTaskActive"] = isActive;
        }

        /// <inheritdoc />
        public bool GetAppVisibilityStatus()
        {
            bool isVisible =
                ApplicationData.Current.LocalSettings.Values.ContainsKey("IsAppVisible") &&
                (bool)ApplicationData.Current.LocalSettings.Values["IsAppVisible"];

            return isVisible;
        }

        /// <inheritdoc />
        public void SetAppVisibilityStatus(bool isVisible)
        {
            ApplicationData.Current.LocalSettings.Values["IsAppVisible"] = isVisible;
        }

        /// <inheritdoc />
        public uint? GetGpsAccuracyExpectation()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("GpsAccuracy"))
            {
                return (uint)ApplicationData.Current.LocalSettings.Values["GpsAccuracy"];
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc />
        public void SetGpsAccuracyExpectation(uint? desiredAccuracyInMeters)
        {
            ApplicationData.Current.LocalSettings.Values["GpsAccuracy"] = desiredAccuracyInMeters;
        }

        /// <inheritdoc />
        public string GetTrackingId()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("TrackingId"))
            {
                return ApplicationData.Current.LocalSettings.Values["TrackingId"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public void SetTrackingId(string trackingId)
        {
            ApplicationData.Current.LocalSettings.Values["TrackingId"] = trackingId;
        }

        /// <inheritdoc />
        public int? GetTrackingMechanismId()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("TrackingMechanismId"))
            {
                return Convert.ToInt32(ApplicationData.Current.LocalSettings.Values["TrackingMechanismId"]);
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc />
        public void SetTrackingMechanism(int trackingMechanismId)
        {
            ApplicationData.Current.LocalSettings.Values["TrackingMechanismId"] = trackingMechanismId;
        }
    }
}

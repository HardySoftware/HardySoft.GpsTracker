namespace HardySoft.GpsTracker.Services.LocalSetting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface to define operations around local settings.
    /// </summary>
    public interface ISettingOperator
    {
        /// <summary>
        /// Resets all settings to original values.
        /// </summary>
        void ResetSettings();

        /// <summary>
        /// Gets the status if the background task is active.
        /// </summary>
        /// <returns>True if background task is active, otherwise false.</returns>
        bool GetBackgroundTaskActiveStatus();

        /// <summary>
        /// Gets the status if the background task is active.
        /// </summary>
        /// <param name="isActive">The status to set.</param>
        void SetBackgroundTaskActiveStatus(bool isActive);

        /// <summary>
        /// Gets the status if the app is visible from setting.
        /// </summary>
        /// <returns>True if app is visible, otherwise false.</returns>
        bool GetAppVisibilityStatus();

        /// <summary>
        /// Sets if the application is visible status to the setting.
        /// </summary>
        /// <param name="isVisible">An indicator whether the app is visible.</param>
        void SetAppVisibilityStatus(bool isVisible);

        /// <summary>
        /// Gets the desired GPS accuracy in meters.
        /// </summary>
        /// <returns>The accuracy in meters.</returns>
        uint? GetGpsAccuracyExpectation();

        /// <summary>
        /// Sets the desired accuracy in meters.
        /// </summary>
        /// <param name="desiredAccuracyInMeters">The accuracy in meters.</param>
        void SetGpsAccuracyExpectation(uint? desiredAccuracyInMeters);

        /// <summary>
        /// Gets the trackingId.
        /// </summary>
        /// <returns>The tracking Id.</returns>
        string GetTrackingId();

        /// <summary>
        /// Sets the tracking Id.
        /// </summary>
        /// <param name="trackingId">The tracking Id to set.</param>
        void SetTrackingId(string trackingId);
    }
}

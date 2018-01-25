namespace HardySoft.GpsTracker.Models
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// An enumeration for all possible activity types supported by the app.
    /// </summary>
    [Serializable]
    public enum ActivityTypes
    {
        /// <summary>
        /// Unknown activity type.
        /// </summary>
        [Description("-")]
        Unknown,

        /// <summary>
        /// Hiking activity.
        /// </summary>
        [Description("Hiking, walking")]
        Hiking,

        /// <summary>
        /// Cycling activity.
        /// </summary>
        [Description("Cycling")]
        Cycling
    }
}

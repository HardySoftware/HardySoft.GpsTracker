namespace HardySoft.GpsTracker.Support.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HardySoft.GpsTracker.Models;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Xaml Combo's SelectedValue won't update UI's selected item automatically.
    /// A converter to manipulate selected index is a work around.
    /// </summary>
    internal class SelectedActivityTypeToIndexConverter : IValueConverter
    {
        /// <summary>
        /// The complete collection of activity type display items.
        /// </summary>
        private IEnumerable<ActivityTypeDisplay> collection = ActivityTypeDisplay.GetAllActivityTypeDisplay();

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && value is ActivityTypes)
            {
                var selectedValue = (ActivityTypes)value;

                var comboDataObject = (from d in this.collection.ToList()
                                       where d.ActivityType == selectedValue
                                       select d).First();

                return this.collection.ToList().IndexOf(comboDataObject);
            }

            return null;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value != null && value is int)
            {
                return this.collection.ToList()[(int)value];
            }

            return null;
        }
    }
}

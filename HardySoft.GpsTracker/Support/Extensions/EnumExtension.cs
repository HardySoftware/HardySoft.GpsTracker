namespace HardySoft.GpsTracker.Support.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension functions for enum type.
    /// </summary>
    internal static class EnumExtension
    {
        /// <summary>
        /// Get description value from each enum item's <see cref="DescriptionAttribute"/>.
        /// </summary>
        /// <param name="value">The enum item.</param>
        /// <returns>The description of the enum item.</returns>
        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            string name = Enum.GetName(type, value);

            if (name != null)
            {
                var field = type.GetField(name);

                if (field != null)
                {
                    var attr = field.GetCustomAttribute<DescriptionAttribute>();

                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }

            return null;
        }
    }
}

namespace HardySoft.GpsTracker.Support.Extensions
{
    using System;
    using System.Reflection;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Extension class for page navigation purpose.
    /// </summary>
    internal static class PageTokenExtension
    {
        /// <summary>
        /// Convert the page type to navigate to page token name.
        /// </summary>
        /// <param name="pageType">The strongly-typed page type.</param>
        /// <returns>The page token supported by Prism.</returns>
        public static string GetPageToken(this Type pageType)
        {
            if (pageType == null)
            {
                throw new ArgumentNullException(nameof(pageType));
            }

            if (!pageType.GetTypeInfo().IsSubclassOf(typeof(Page)))
            {
                throw new InvalidOperationException("Only sub type of Page can be used for page type.");
            }

            var typeName = pageType.Name;

            if (!typeName.EndsWith("Page", StringComparison.Ordinal))
            {
                throw new ArgumentException("Only view ending with \"Page\" could be used here.");
            }

            return typeName.Replace("Page", string.Empty);
        }
    }
}

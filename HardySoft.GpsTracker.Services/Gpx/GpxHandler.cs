namespace HardySoft.GpsTracker.Services.Gpx
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Windows.Devices.Geolocation;
    using Windows.Storage;

    /// <summary>
    /// Class to implement <see cref="IGpxHandler"/> to provide GPX file handling.
    /// </summary>
    public class GpxHandler : IGpxHandler
    {
        /// <summary>
        /// The Xml section template for a way-point.
        /// </summary>
        /// <remarks>It is based on GPX 1.1 schema.</remarks>
        private const string WaypointXmlTemplate = "<trkpt lat=\"{0}\" lon=\"{1}\"><ele>{2}</ele><time>{3}</time><desc>position source {4}, accuracy {5}.</desc></trkpt>";

        /// <inheritdoc />
        public async Task RecordLocationAsync(string trackingId, Geocoordinate coordinate)
        {
            if (string.IsNullOrWhiteSpace(trackingId))
            {
                return;
            }

            var waypointSection = string.Format(
                WaypointXmlTemplate,
                coordinate.Point.Position.Latitude,
                coordinate.Point.Position.Longitude,
                coordinate.Point.Position.Altitude,
                DateTime.Now.ToUniversalTime().ToString("o"),
                coordinate.PositionSource,
                coordinate.Accuracy);

            var sectionFileName = trackingId + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";

            var storageFolder = ApplicationData.Current.LocalFolder;

            var workingFolderName = "working";
            var possibleWorkingFolder = await storageFolder.TryGetItemAsync(workingFolderName);
            if (possibleWorkingFolder == null)
            {
                await storageFolder.CreateFolderAsync(workingFolderName);
            }

            var workingFolder = await storageFolder.GetFolderAsync(workingFolderName);

            var waypointFile = await workingFolder.CreateFileAsync(sectionFileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(waypointFile, waypointSection);
        }
    }
}

namespace HardySoft.GpsTracker.Services.Gpx
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.HockeyApp;
    using Polly;
    using Polly.Retry;
    using Windows.Devices.Geolocation;
    using Windows.Storage;
    using Windows.Storage.Search;

    /// <summary>
    /// Class to implement <see cref="IGpxHandler"/> to provide GPX file handling.
    /// </summary>
    public class GpxHandler : IGpxHandler
    {
        /// <summary>
        /// The Xml template for a complete GPX file.
        /// </summary>
        private const string GpxXmlTemplate = @"<gpx xmlns=""http://www.topografix.com/GPX/1/1""
	xmlns:gpxx=""http://www.garmin.com/xmlschemas/GpxExtensions/v3""
	xmlns:gpxtpx=""http://www.garmin.com/xmlschemas/TrackPointExtension/v1""
	creator=""Track my movement""
	version=""1.1""
	xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
	xsi:schemaLocation=""http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd http://www.garmin.com/xmlschemas/GpxExtensions/v3 http://www.garmin.com/xmlschemas/GpxExtensionsv3.xsd http://www.garmin.com/xmlschemas/TrackPointExtension/v1 http://www.garmin.com/xmlschemas/TrackPointExtensionv1.xsd"">
  <metadata>
    <link href=""https://github.com/HardySoftware/track-my-movement"">
      <text>Hardy Software Track my movement</text>
    </link>
    <time>{0}</time>
  </metadata>
  <trk>
    <name>{1}</name>
    <trkseg>
{2}
    </trkseg>
  </trk>
</gpx>";

        /// <summary>
        /// The Xml section template for a single way-point section in GPX file.
        /// </summary>
        /// <remarks>It is based on GPX 1.1 schema.</remarks>
        private const string WaypointXmlTemplate = "<trkpt lat=\"{0}\" lon=\"{1}\"><ele>{2}</ele><time>{3}</time><desc>position source {4}, accuracy {5}. Additional comment {6}</desc></trkpt>";

        /// <summary>
        /// The Xml comment section template.
        /// </summary>
        private const string XmlCommentTemplate = "<!--{0}-->";

        /// <summary>
        /// The working folder name for way-point section files.
        /// </summary>
        private const string WorkingFolderName = "working";

        /// <summary>
        /// The final folder name for GPX files.
        /// </summary>
        private const string GpxFolderName = "gpx";

        /// <summary>
        /// Retry policy definition for creating way point file.
        /// </summary>
        private readonly RetryPolicy waypointFileRetryPolicy;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpxHandler"/> class.
        /// </summary>
        public GpxHandler()
        {
            this.waypointFileRetryPolicy = Policy
                .Handle<FileLoadException>()
                .WaitAndRetryAsync(
                5,
                retryCount => TimeSpan.FromSeconds(retryCount),
                (exception, timespan) =>
                {
                    Debug.WriteLine($"{DateTime.Now} - waypointFileRetryPolicy retry. {exception.Message}");
                    HockeyClient.Current.TrackException(exception, new Dictionary<string, string>() { { "Polly Exception Retry", string.Empty } });
                });
        }

        /// <inheritdoc />
        public async Task RecordLocationAsync(string trackingId, Geocoordinate coordinate, string comment)
        {
            if (string.IsNullOrWhiteSpace(trackingId))
            {
                return;
            }

            if (coordinate == null)
            {
                return;
            }

            var waypointSectionContent = string.Format(
                WaypointXmlTemplate,
                coordinate.Point.Position.Latitude,
                coordinate.Point.Position.Longitude,
                coordinate.Point.Position.Altitude,
                DateTime.Now.ToUniversalTime().ToString("o"),
                coordinate.PositionSource,
                coordinate.Accuracy,
                comment) + "\r\n";

            var workingFolder = await GetFolder(WorkingFolderName);

            var randomFileName = GetRandomString(6);

            var sectionFileName = $"{trackingId}-{DateTime.Now.ToString("yyyyMMddHHmmss")}-{randomFileName}.xml";
            Debug.WriteLine($"{DateTime.Now} - Creating way-point section file {sectionFileName}");
            var waypointFile = await workingFolder.CreateFileAsync(sectionFileName, CreationCollisionOption.ReplaceExisting);

            await this.waypointFileRetryPolicy.ExecuteAsync(async () => await FileIO.WriteTextAsync(waypointFile, waypointSectionContent));
        }

        /// <inheritdoc />
        public async Task RecordCommentAsync(string trackingId, string comment)
        {
            if (string.IsNullOrWhiteSpace(trackingId))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                return;
            }

            var randomFileName = GetRandomString(6);

            var commentContent = string.Format(XmlCommentTemplate, comment);

            var workingFolder = await GetFolder(WorkingFolderName);

            var sectionFileName = $"{trackingId}-{DateTime.Now.ToString("yyyyMMddHHmmss")}-Comment-{randomFileName}.xml";
            var commentFile = await workingFolder.CreateFileAsync(sectionFileName, CreationCollisionOption.ReplaceExisting);

            await this.waypointFileRetryPolicy.ExecuteAsync(async () => await FileIO.WriteTextAsync(commentFile, commentContent));
        }

        /// <inheritdoc />
        public async Task<int> ComposeGpxFile(string trackingId, string activityName)
        {
            if (string.IsNullOrWhiteSpace(trackingId))
            {
                return 0;
            }

            var gpxFileName = $"{trackingId}.xml";
            HockeyClient.Current.TrackEvent("Composing GPX file", new Dictionary<string, string> { { "File Name", gpxFileName } }, null);

            var workingFolder = await GetFolder(WorkingFolderName);

            var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, new List<string> { $".xml" })
            {
                UserSearchFilter = $"{trackingId}"
            };

            var queryResult = workingFolder.CreateFileQueryWithOptions(queryOptions);
            var gpxWaypointFiles = await queryResult.GetFilesAsync();

            var sb = new StringBuilder();
            int commentFileCounter = 0;
            foreach (var file in gpxWaypointFiles)
            {
                if (file.Name.Contains("-Comment"))
                {
                    commentFileCounter++;
                }

                var waypointXml = await FileIO.ReadTextAsync(file);
                sb.Append(waypointXml);
            }

            var gpxXml = string.Format(GpxXmlTemplate, DateTime.Now.ToUniversalTime().ToString("o"), activityName, sb);
            var gpxFolder = await GetFolder(GpxFolderName);
            var gpxFile = await gpxFolder.CreateFileAsync(gpxFileName, CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(gpxFile, gpxXml);

            // Delete working files after GPX file is created
            foreach (var file in gpxWaypointFiles)
            {
                await this.waypointFileRetryPolicy.ExecuteAsync(async () => await file.DeleteAsync());
            }

            HockeyClient.Current.TrackEvent("Composed GPX file", new Dictionary<string, string> { { "File Name", gpxFileName } }, null);

            return gpxWaypointFiles.Count - commentFileCounter;
        }

        /// <inheritdoc />
        public async Task<KeyValuePair<int, ulong>> ClearTemporaryGpxWaypointFiles()
        {
            var workingFolder = await GetFolder(WorkingFolderName);
            var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, new List<string> { $".xml" });
            var queryResult = workingFolder.CreateFileQueryWithOptions(queryOptions);
            var gpxWaypointFiles = await queryResult.GetFilesAsync();

            int fileCounter = 0;
            ulong totalFileSize = 0;

            foreach (var file in gpxWaypointFiles)
            {
                var size = await GetFileSizeAsync(file);
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);

                fileCounter++;
                totalFileSize += size;
            }

            return new KeyValuePair<int, ulong>(fileCounter, totalFileSize);
        }

        /// <summary>
        /// Make sure the designated folder exists, and return it.
        /// </summary>
        /// <param name="folderName">The name of the folder.</param>
        /// <returns>Storage folder object representing the folder name.</returns>
        private static async Task<StorageFolder> GetFolder(string folderName)
        {
            var storageFolder = ApplicationData.Current.LocalFolder;

            var possibleFolder = await storageFolder.TryGetItemAsync(folderName);
            if (possibleFolder == null)
            {
                await storageFolder.CreateFolderAsync(folderName);
            }

            var folder = await storageFolder.GetFolderAsync(folderName);

            return folder;
        }

        /// <summary>
        /// Gets the size of storage file.
        /// </summary>
        /// <param name="file">The storage file.</param>
        /// <returns>The storage file size in bytes.</returns>
        private static async Task<ulong> GetFileSizeAsync(StorageFile file)
        {
            var properties = await file.GetBasicPropertiesAsync();
            return properties.Size;
        }

        /// <summary>
        /// Generates a random string.
        /// </summary>
        /// <param name="length">The length of random string.</param>
        /// <returns>The generated random string.</returns>
        private static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var random = new Random();

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
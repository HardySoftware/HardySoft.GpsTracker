namespace HardySoft.GpsTracker.Services.Gpx.Models
{
    /// <summary>
    /// A delegate to allow the consumer to receive update of the location tracking status.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="statusUpdate">The location tracking status to report to the event handler.</param>
    public delegate void UpdateTrackingProgress(object sender, LocationResponseEventArgs statusUpdate);
}

using Windows.ApplicationModel.Background;
using Floodlight.Client.Common;
using Floodlight.Client.Managers;

namespace Floodlight.Background
{
    /// <summary>
    /// Automatic task which runs in the background, downloading and updating backgrounds.
    /// </summary>
    public sealed class BackgroundChanger : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            //Initialize Telemetry
            TelemetryManager.InitializeBackgroundTelemetry();

            TelemetryManager.TrackEvent("Starting Background Downloader task...");
            await Downloader.Execute();

            TelemetryManager.TrackEvent("Starting Background Changer task...");
            await Changer.Execute();
            TelemetryManager.TrackEvent("Background tasks finished!");

            deferral.Complete();
        }
    }
}

using Windows.ApplicationModel.Background;
using Floodlight.Client.Common;
using Floodlight.Client.Managers;

namespace Floodlight.Background
{
    public sealed class BackgroundDownloader : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            //Initialize Telemetry
            TelemetryManager.InitializeBackgroundTelemetry();

            TelemetryManager.TrackEvent("Starting Background Downloader task...");
            await Downloader.Execute();
            TelemetryManager.TrackEvent("Background Downloader task finished!");

            deferral.Complete();
        }
    }
}

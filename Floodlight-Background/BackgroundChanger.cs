using Windows.ApplicationModel.Background;
using Floodlight.Client.Common;
using Floodlight.Client.Managers;

namespace Floodlight.Background
{
    public sealed class BackgroundChanger : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            //Initialize Telemetry
            TelemetryManager.InitializeBackgroundTelemetry();

            TelemetryManager.TrackEvent("Starting Background Changer task...");
            await Changer.Execute();
            TelemetryManager.TrackEvent("Background Changer task finished!");

            deferral.Complete();
        }
    }
}

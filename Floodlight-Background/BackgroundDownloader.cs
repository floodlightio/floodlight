using Windows.ApplicationModel.Background;
using Floodlight.Client.Common;

namespace Floodlight.Background
{
    public sealed class BackgroundDownloader : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            await Downloader.Execute();
            deferral.Complete();
        }
    }
}

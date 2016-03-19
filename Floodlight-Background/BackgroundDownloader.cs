using Windows.ApplicationModel.Background;

namespace Floodlight.Background
{
    public sealed class BackgroundDownloader : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            Client.Common.Changer.Execute();
            deferral.Complete();
        }
    }
}

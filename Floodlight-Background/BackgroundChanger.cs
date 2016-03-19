using Windows.ApplicationModel.Background;
using Floodlight.Client;

namespace Floodlight.Background
{
    public sealed class BackgroundChanger : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            CommonChanger.Execute();
            deferral.Complete();
        }
    }
}

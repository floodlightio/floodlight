using Windows.ApplicationModel.Background;
using Floodlight.Client;
using Floodlight.Client.Common;

namespace Floodlight.Background
{
    public sealed class BackgroundChanger : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            Client.Common.BackgroundChanger.Execute();
            deferral.Complete();
        }
    }
}

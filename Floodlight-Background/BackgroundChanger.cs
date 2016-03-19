using Windows.ApplicationModel.Background;
using Floodlight.Client.Common;

namespace Floodlight.Background
{
    public sealed class BackgroundChanger : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            Changer.Execute();
            deferral.Complete();
        }
    }
}

using Windows.ApplicationModel.Background;
using Floodlight.Client.Common;

namespace Floodlight.Background
{
    public sealed class BackgroundChanger : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            await Changer.Execute();
            deferral.Complete();
        }
    }
}

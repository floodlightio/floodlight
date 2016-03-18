using Windows.ApplicationModel.Background;

namespace BGChanger.Client.BackgroundTasks
{
    public sealed class BackgroundDownloader : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            CommonUpdater.GetAndSaveImages();
            deferral.Complete();
        }
    }
}

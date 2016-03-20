using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Floodlight.Client.Managers
{
    public static class BackgroundManager
    {
        private const string ChangerTaskName = "Floodlight Background Changer";
        private const string ChangerTaskEntry = "Floodlight.Background.BackgroundChanger";
        private const string DownloaderTaskName = "Floodlight Background Downloader";
        private const string DownloaderTaskEntry = "Floodlight.Background.BackgroundDownloader";

        public static async Task RegisterAllTasks()
        {
            await BackgroundExecutionManager.RequestAccessAsync();

            RegisterTask(ChangerTaskName, ChangerTaskEntry, 30);
            RegisterTask(DownloaderTaskName, DownloaderTaskEntry, 15);
        }

        private static IBackgroundTaskRegistration GetTask(string taskName)
        {
            return BackgroundTaskRegistration.AllTasks.FirstOrDefault(pair => pair.Value.Name == taskName).Value;
        }

        private static void RegisterTask(string taskName, string taskEntry, uint frequency)
        {
            if (GetTask(taskName) == null)
            {
                var taskBuilder = new BackgroundTaskBuilder
                {
                    Name = taskName,
                    TaskEntryPoint = taskEntry
                };

                taskBuilder.SetTrigger(new TimeTrigger(frequency, false));
                taskBuilder.SetTrigger(new MaintenanceTrigger(frequency, false));
                taskBuilder.Register();
            }
        }

        private static void UnregisterTask(string taskName)
        {
            GetTask(taskName)?.Unregister(true);
        }
    }
}

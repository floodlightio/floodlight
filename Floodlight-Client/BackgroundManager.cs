using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Floodlight.Client
{
    public static class BackgroundManager
    {
        private const string ChangerTaskName = "Floodlight Background Changer";
        private const string ChangerTaskEntry = "Floodlight.Background.BackgroundChanger";
        private const string UpdaterTaskName = "Floodlight Background Downloader";
        private const string UpdaterTaskEntry = "Floodlight.Background.BackgroundDownloader";

        public static async void RegisterAllTasks()
        {
            await BackgroundExecutionManager.RequestAccessAsync();

            // Remove old tasks
            UnregisterTask(ChangerTaskName);
            UnregisterTask(UpdaterTaskName);

            // Add new tasks
            RegisterTask(ChangerTaskName, ChangerTaskEntry, 30);
            RegisterTask(UpdaterTaskName, UpdaterTaskEntry, 15);
        }

        private static IBackgroundTaskRegistration GetTask(string taskName)
        {
            return BackgroundTaskRegistration.AllTasks.FirstOrDefault(pair => pair.Value.Name == taskName).Value;
        }

        private static void RegisterTask(string taskName, string taskEntry, uint frequency)
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

        private static void UnregisterTask(string taskName)
        {
            var task = GetTask(taskName);

            if (task != null)
            {
                BackgroundTaskRegistration.AllTasks.First(pair => pair.Value.Name == taskName).Value.Unregister(true);
            }
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Floodlight.Client.Managers
{
    /// <summary>
    /// Manages the background tasks for the app.
    /// </summary>
    public static class BackgroundManager
    {
        private const string ChangerTaskName = "Floodlight Background Changer";
        private const string ChangerTaskEntry = "Floodlight.Background.BackgroundChanger";
        
        /// <summary>
        /// Register all background tasks for the app.
        /// </summary>
        public static async Task RegisterAllTasks()
        {
            await BackgroundExecutionManager.RequestAccessAsync();

            RegisterTask(ChangerTaskName, ChangerTaskEntry, 15);
        }

        /// <summary>
        /// Get the task registration from the registration list.
        /// </summary>
        /// <param name="taskName">The name of the task to get</param>
        /// <returns>The task registration requested.</returns>
        private static IBackgroundTaskRegistration GetTask(string taskName)
        {
            return BackgroundTaskRegistration.AllTasks.FirstOrDefault(pair => pair.Value.Name == taskName).Value;
        }

        /// <summary>
        /// Register a new task with specified name, entry point and frequency.
        /// </summary>
        /// <param name="taskName">The name of the task to register.</param>
        /// <param name="taskEntry">The entry point of the task, in the form [Namespace].[Class].</param>
        /// <param name="frequency">The frequency that the task needs to run.</param>
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
                taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                taskBuilder.AddCondition(new SystemCondition(SystemConditionType.SessionConnected));
                taskBuilder.Register();
            }
        }

        /// <summary>
        /// Unregister the specified task.
        /// </summary>
        /// <param name="taskName">The name of the task to unregister.</param>
        private static void UnregisterTask(string taskName)
        {
            GetTask(taskName)?.Unregister(true);
        }
    }
}

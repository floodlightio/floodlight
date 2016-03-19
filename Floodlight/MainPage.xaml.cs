using System;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Floodlight.Client;

namespace Floodlight
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            InitializeBackgroundTasks();
            UpdateValues();
            CommonUpdater.GetAndSaveImages();
        }

        private async void InitializeBackgroundTasks()
        {
            const string changerTaskName = "Floodlight Background Changer";
            const string updaterTaskName = "Floodlight Background Downloader";

            await BackgroundExecutionManager.RequestAccessAsync();

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                task.Value.Unregister(true);
            }

            var changerBuilder = new BackgroundTaskBuilder
            {
                Name = changerTaskName,
                TaskEntryPoint = "Floodlight.Background.BackgroundChanger"
            };

            changerBuilder.SetTrigger(new TimeTrigger(30, false));
            changerBuilder.SetTrigger(new MaintenanceTrigger(30, false));
            changerBuilder.Register();
            
            var updaterBuilder = new BackgroundTaskBuilder
            {
                Name = updaterTaskName,
                TaskEntryPoint = "Floodlight.Background.BackgroundDownloader"
            };

            updaterBuilder.SetTrigger(new TimeTrigger(15, false));
            updaterBuilder.SetTrigger(new MaintenanceTrigger(15, false));
            updaterBuilder.Register();
            
        }

        private void UpdateValues()
        {
            ServerUrl.Text = SettingsManager.ServerAddress;
            ApiKey.Text = SettingsManager.UserId;
            LastUpdatedDate.Text = SettingsManager.LastUpdatedDate.ToString(CultureInfo.InvariantCulture);

            UpdateWallpaper.IsOn = SettingsManager.UpdateWallpaper;
            UpdateLockScreen.IsOn = SettingsManager.UpdateLockScreen;
        }

        /**
         * Event Handlers
         */
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.IsPaneOpen = !MainContent.IsPaneOpen;
        }

        private void UpdateWallpaper_OnToggled(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            SettingsManager.UpdateWallpaper = toggle.IsOn;
        }

        private void UpdateLockScreen_OnToggled(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            SettingsManager.UpdateLockScreen = toggle.IsOn;
        }
    }
}

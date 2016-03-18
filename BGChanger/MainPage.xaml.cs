﻿using System;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BGChanger.Client;
using BGChanger.Client.BackgroundTasks;

namespace BGChanger
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
                TaskEntryPoint = "BGChanger.Client.BackgroundTasks.BackgroundChanger"
            };

            changerBuilder.SetTrigger(new TimeTrigger(30, false));
            changerBuilder.SetTrigger(new MaintenanceTrigger(30, false));
            changerBuilder.Register();
            
            var updaterBuilder = new BackgroundTaskBuilder
            {
                Name = updaterTaskName,
                TaskEntryPoint = "BGChanger.Client.BackgroundTasks.BackgroundDownloader"
            };

            updaterBuilder.SetTrigger(new TimeTrigger(15, false));
            updaterBuilder.SetTrigger(new MaintenanceTrigger(15, false));
            updaterBuilder.Register();
            
        }

        private void UpdateValues()
        {
            ServerUrl.Text = SettingsHelper.ServerAddress;
            ApiKey.Text = SettingsHelper.UserId;
            LastUpdatedDate.Text = SettingsHelper.LastUpdatedDate.ToString(CultureInfo.InvariantCulture);

            UpdateWallpaper.IsOn = SettingsHelper.UpdateWallpaper;
            UpdateLockScreen.IsOn = SettingsHelper.UpdateLockScreen;
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
            SettingsHelper.UpdateWallpaper = toggle.IsOn;
        }

        private void UpdateLockScreen_OnToggled(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            SettingsHelper.UpdateLockScreen = toggle.IsOn;
        }
    }
}
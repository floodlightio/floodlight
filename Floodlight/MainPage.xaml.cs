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
            BackgroundManager.RegisterAllTasks();
            CommonUpdater.GetAndSaveImages();
            UpdateValues();
        }

        private void UpdateValues()
        {
            ServerUrl.Text = SettingsManager.UserDefined.ServerAddress;
            ApiKey.Text = SettingsManager.UserDefined.UserId;
            LastUpdatedDate.Text = SettingsManager.Internal.LastUpdatedDate.ToString(CultureInfo.InvariantCulture);

            UpdateWallpaper.IsOn = SettingsManager.UserDefined.UpdateWallpaper;
            UpdateLockScreen.IsOn = SettingsManager.UserDefined.UpdateLockScreen;
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
            SettingsManager.UserDefined.UpdateWallpaper = toggle.IsOn;
        }

        private void UpdateLockScreen_OnToggled(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            SettingsManager.UserDefined.UpdateLockScreen = toggle.IsOn;
        }
    }
}

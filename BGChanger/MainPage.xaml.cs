using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BGChanger.Client;

namespace BGChanger
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            UpdateValues();
        }

        public void UpdateValues()
        {
            ServerUrl.Text = SettingsHelper.Url;
            ApiKey.Text = SettingsHelper.ApiKey;
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

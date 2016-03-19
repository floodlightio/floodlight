using System.Globalization;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Floodlight.Client;

namespace Floodlight
{
    /// <summary>
    /// The Settings page for Floodlight
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += Settings_BackRequested;

            InitializeFormValues();
        }

        private void InitializeFormValues()
        {
            var currentWallpaper = SettingsManager.Internal.CurrentWallpaper;
            var currentLockScreen = SettingsManager.Internal.CurrentLockScreen;

            // User Defined Settings
            ServerAddress.Text = SettingsManager.UserDefined.ServerAddress;
            UserId.Text = SettingsManager.UserDefined.UserId;
            UpdateWallpaper.IsOn = SettingsManager.UserDefined.UpdateWallpaper;
            UpdateLockScreen.IsOn = SettingsManager.UserDefined.UpdateLockScreen;
            UseSameImage.IsOn = SettingsManager.UserDefined.UseSameImage;

            // Internal Settings
            LastRetrieved.Text = SettingsManager.Internal.LastRetrievedDate.ToString(CultureInfo.InvariantCulture);
            LastUpdated.Text = SettingsManager.Internal.LastUpdatedDate.ToString(CultureInfo.InvariantCulture);
            WallpaperTitle.Text = currentWallpaper != null ? currentWallpaper.Title : "Unknown";
            LockScreenTitle.Text = currentLockScreen != null ? currentLockScreen.Title : "Unknown";
        }

        private void Settings_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                e.Handled = true;
            }
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

        private void UseSameImage_OnToggled(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            SettingsManager.UserDefined.UseSameImage = toggle.IsOn;
        }
    }
}

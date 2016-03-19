using System.Globalization;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Floodlight.Client.Managers;

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
            ServiceAddress.Text = SettingsManager.UserDefined.ServiceAddress;
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

        private void ServiceAddress_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            SettingsManager.UserDefined.ServiceAddress = ServiceAddress.Text;
        }

        private void UserId_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            SettingsManager.UserDefined.UserId = UserId.Text;
        }

        private void UpdateWallpaper_OnToggled(object sender, RoutedEventArgs e)
        {
            SettingsManager.UserDefined.UpdateWallpaper = UpdateWallpaper.IsOn;
        }

        private void UpdateLockScreen_OnToggled(object sender, RoutedEventArgs e)
        {
            SettingsManager.UserDefined.UpdateLockScreen = UpdateLockScreen.IsOn;
        }

        private void UseSameImage_OnToggled(object sender, RoutedEventArgs e)
        {
            SettingsManager.UserDefined.UseSameImage = UseSameImage.IsOn;
        }
    }
}

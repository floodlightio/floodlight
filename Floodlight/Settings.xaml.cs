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
        /// <summary>
        /// Entry point for the Settings page.
        /// </summary>
        public Settings()
        {
            InitializeComponent();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += Settings_BackRequested;

            InitializeFormValues();
        }

        /// <summary>
        /// Initialize all values for form elements.
        /// </summary>
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

        /// <summary>
        /// Go back to the main page of the app.
        /// </summary>
        private void Settings_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event fired when the Service address was changed.
        /// </summary>
        private void ServiceAddress_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            SettingsManager.UserDefined.ServiceAddress = ServiceAddress.Text;
        }

        /// <summary>
        /// Event fired when the User ID was changed.
        /// </summary>
        private void UserId_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            SettingsManager.UserDefined.UserId = UserId.Text;
        }

        /// <summary>
        /// Event fired when the "Update wallpaper" option was toggled.
        /// </summary>
        private void UpdateWallpaper_OnToggled(object sender, RoutedEventArgs e)
        {
            SettingsManager.UserDefined.UpdateWallpaper = UpdateWallpaper.IsOn;
        }

        /// <summary>
        /// Event fired when the "Update lock screen" option was toggled.
        /// </summary>
        private void UpdateLockScreen_OnToggled(object sender, RoutedEventArgs e)
        {
            SettingsManager.UserDefined.UpdateLockScreen = UpdateLockScreen.IsOn;
        }

        /// <summary>
        /// Event fired when the "Use same image" option was toggled.
        /// </summary>
        private void UseSameImage_OnToggled(object sender, RoutedEventArgs e)
        {
            SettingsManager.UserDefined.UseSameImage = UseSameImage.IsOn;
        }
    }
}

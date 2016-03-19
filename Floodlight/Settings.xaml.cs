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
            this.InitializeComponent();
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

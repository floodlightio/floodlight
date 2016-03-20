#pragma warning disable 4014 // Disabling async warnings

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Floodlight.Client.Common;
using Floodlight.Client.Managers;
using HockeyApp;

namespace Floodlight
{
    /// <summary>
    /// The main page for Floodlight, showing a grid of images.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// Entry point for the main page.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            BackgroundManager.RegisterAllTasks();
            Downloader.Execute();

#if DEBUG
            CreateDebugCommands();
#endif
        }

        /// <summary>
        /// Create commands used for debugging purposes.
        /// </summary>
        private void CreateDebugCommands()
        {
            var clearCacheCommand = new AppBarButton
            {
                Icon = new SymbolIcon(Symbol.Delete),
                Label = "Clear cache"
            };
            clearCacheCommand.Click += (sender, args) => SettingsManager.Internal.ClearBackgroundCache();

            var forceResyncCommand = new AppBarButton()
            {
                Icon = new SymbolIcon(Symbol.SyncFolder),
                Label = "Force Resync"
            };
            forceResyncCommand.Click += (sender, args) => Downloader.Execute(false);

            AppCommands.PrimaryCommands.Add(new AppBarSeparator());
            AppCommands.PrimaryCommands.Add(clearCacheCommand);
            AppCommands.PrimaryCommands.Add(forceResyncCommand);
        }

        /// <summary>
        /// Force a download of the background metadata from the Floodlight service.
        /// </summary>
        private void Download_OnClick(object sender, RoutedEventArgs e)
        {
            Downloader.Execute();
        }

        /// <summary>
        /// Force a wallpaper/lock screen change.
        /// </summary>
        private void Change_OnClick(object sender, RoutedEventArgs e)
        {
            Changer.Execute();
        }

        /// <summary>
        /// Go to the Settings page.
        /// </summary>
        private void Settings_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Settings));
        }

        /// <summary>
        /// Open up the HockeyApp feedback pane.
        /// </summary>
        private void Feedback_OnClick_OnClick(object sender, RoutedEventArgs e)
        {
            HockeyClient.Current.ShowFeedback();
        }
    }
}

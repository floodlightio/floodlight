using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Floodlight.Client.Common;
using Floodlight.Client.Managers;
using HockeyApp;

namespace Floodlight
{
    public sealed partial class MainPage
    {
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

        private void Download_OnClick(object sender, RoutedEventArgs e)
        {
            Downloader.Execute();
        }

        private void Change_OnClick(object sender, RoutedEventArgs e)
        {
            Changer.Execute();
        }

        private void Settings_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Settings));
        }

        private void Feedback_OnClick_OnClick(object sender, RoutedEventArgs e)
        {
            HockeyClient.Current.ShowFeedback();
        }
    }
}

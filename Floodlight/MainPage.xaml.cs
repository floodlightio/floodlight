using Windows.UI.Core;
using Windows.UI.Xaml;
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

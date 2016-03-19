using System;
using System.Globalization;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
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
            CommonDownloader.Execute();
        }

        private void Download_OnClick(object sender, RoutedEventArgs e)
        {
            CommonDownloader.Execute();
        }

        private void Change_OnClick(object sender, RoutedEventArgs e)
        {
            CommonChanger.Execute();
        }

        private void Settings_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Settings));
        }
    }
}

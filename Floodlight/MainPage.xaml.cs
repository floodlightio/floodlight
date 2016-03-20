#pragma warning disable 4014 // Disabling async warnings

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
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
        public readonly List<GridImage> ImagesAvailable;
        public GridImage SelectedImage;

        /// <summary>
        /// Entry point for the main page.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            ImagesAvailable = SettingsManager.Internal.GetBackgroundCache().Select(pair => new GridImage(pair.Value)).ToList();

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
        /// Resizes the grid based on the current size.
        /// </summary>
        private void AvailableImagesGrid_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var panel = (ItemsWrapGrid)AvailableImagesGrid.ItemsPanelRoot;

            if (panel != null)
            {
                panel.ItemWidth = e.NewSize.Width / 4;
                panel.ItemHeight = e.NewSize.Height / 4;
            }
        }

        /// <summary>
        /// Hide and show the current selection based on selection.
        /// </summary>
        private void AvailableImagesGrid_OnSelectionChanged(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            if (!(tappedRoutedEventArgs.OriginalSource is Image)) return;

            var selectedImage = (GridImage)AvailableImagesGrid.SelectedItem;
            if (SelectedImage == null || SelectedImage != selectedImage)
            {
                CurrentSelectedImage.Source = new BitmapImage(new Uri(selectedImage.Path));
                CurrentSelectedTitle.Text = selectedImage.Title;
                CurrentSelectedPanel.Visibility = Visibility.Visible;
                SelectedImage = selectedImage;
            }
            else
            {
                AvailableImagesGrid.SelectedIndex = -1;
                CurrentSelectedPanel.Visibility = Visibility.Collapsed;
                SelectedImage = null;
            }
        }

        /// <summary>
        /// Change wallpaper to currently selected image.
        /// </summary>
        private void CurrentWallpaper_OnClick(object sender, RoutedEventArgs e)
        {
            Changer.ChangeWallpaper(SelectedImage.Metadata, true);
        }

        /// <summary>
        /// Change lock screen to currently selected image.
        /// </summary>
        private void CurrentLockScreen_OnClick(object sender, RoutedEventArgs e)
        {
            Changer.ChangeLockScreen(SelectedImage.Metadata, true);
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

    public class GridImage
    {
        public Client.Models.Background Metadata;
        public string Path;
        public string Title;

        public GridImage(Client.Models.Background bg)
        {
            Path = FileManager.GetFullPathFromBackground(bg);
            Title = bg.Title;
            Metadata = bg;
        }
    }
}

using System;
using System.Linq;
using Windows.System.UserProfile;
using Floodlight.Client.Models;

namespace Floodlight.Client
{
    public static class CommonChanger
    {
        public static void Execute()
        {
            var cachedBackgrounds = SettingsManager.Internal.GetBackgroundCache();
            var bgIndex = new Random().Next(cachedBackgrounds.Count - 1);
            var background = cachedBackgrounds.Values.ElementAt(bgIndex);

            ChangeWallpaper(background);
            ChangeLockScreen(background);

            SettingsManager.Internal.LastUpdatedDate = DateTime.Now;
        }

        private static async void ChangeWallpaper(Background background)
        {
            var backgroundFile = await FileManager.GetBackgroundFromLocalFolder(background);
            await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(backgroundFile);
        }

        private static async void ChangeLockScreen(Background background)
        {
            
            var backgroundFile = await FileManager.GetBackgroundFromLocalFolder(background);
            await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(backgroundFile);
        }

    }
}

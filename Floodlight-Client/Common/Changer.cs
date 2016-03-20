using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using Floodlight.Client.Managers;
using Floodlight.Client.Models;

namespace Floodlight.Client.Common
{
    /// <summary>
    /// Provides common functionality around changing the wallpaper and lock screen.
    /// </summary>
    public static class Changer
    {
        /// <summary>
        /// Run a single iteration of the changing cycle:
        ///  - Get list of cached backgrounds
        ///  - Retrieve background from file for each of wallpaper and lock screen
        ///  - Set the wallpaper and lock screen images
        /// </summary>
        public static async Task Execute()
        {
            var backgroundCache = SettingsManager.Internal.GetBackgroundCache();
            Background wallpaperBackground;
            Background lockScreenBackground;

            if (SettingsManager.UserDefined.UseSameImage)
            {
                var sharedBackground = GetRandomBackground(backgroundCache);
                wallpaperBackground = sharedBackground;
                lockScreenBackground = sharedBackground;
            }
            else
            {
                wallpaperBackground = GetRandomBackground(backgroundCache);
                lockScreenBackground = GetRandomBackground(backgroundCache);
            }

            if (SettingsManager.UserDefined.UpdateWallpaper)
            {
                TelemetryManager.TrackEvent("Updating Wallpaper...", 
                    new Dictionary<string, string>() { { "backgroundId", wallpaperBackground.Id } });
                await ChangeWallpaper(wallpaperBackground);
            }

            if (SettingsManager.UserDefined.UpdateLockScreen)
            {
                TelemetryManager.TrackEvent("Updating Lock Screen...",
                    new Dictionary<string, string>() { { "backgroundId", wallpaperBackground.Id } });
                await ChangeLockScreen(lockScreenBackground);
            }

            // Update the last updated date if either was changed
            if (SettingsManager.UserDefined.UpdateWallpaper || SettingsManager.UserDefined.UpdateLockScreen)
            {
                SettingsManager.Internal.LastUpdatedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Gets a random background from the background cache.
        /// </summary>
        /// <param name="backgroundCache">The background cache to use for this operation.</param>
        /// <returns>A background from the cache, chosen at random.</returns>
        private static Background GetRandomBackground(Dictionary<string, Background> backgroundCache)
        {
            var bgIndex = new Random().Next(backgroundCache.Count);
            return backgroundCache.Values.ElementAt(bgIndex);
        }

        /// <summary>
        /// Change the wallpaper to the one specified in the provided metadata.
        /// </summary>
        /// <param name="background">The background to use.</param>
       private static async Task ChangeWallpaper(Background background)
        {
            var backgroundFile = await FileManager.GetBackgroundFromLocalFolder(background);

            if (backgroundFile != null)
            {
                await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(backgroundFile);
            }
            else
            {
                TelemetryManager.TrackEvent("Background could not be found in file cache!", new Dictionary<string, string>()
                {
                    { "backgroundId", background.Id }
                });
            }

            SettingsManager.Internal.CurrentWallpaper = background;
        }

        /// <summary>
        /// Change the lock screen to the one specified in the provided metadata.
        /// </summary>
        /// <param name="background">The background to use.</param>
        private static async Task ChangeLockScreen(Background background)
        {
            
            var backgroundFile = await FileManager.GetBackgroundFromLocalFolder(background);

            if (backgroundFile != null)
            {
                await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(backgroundFile);
            }
            else
            {
                TelemetryManager.TrackEvent("Background could not be found in file cache!", new Dictionary<string, string>()
                {
                    { "backgroundId", background.Id }
                });
            }

            SettingsManager.Internal.CurrentLockScreen = background;
        }

    }
}

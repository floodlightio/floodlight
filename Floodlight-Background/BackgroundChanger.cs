using System;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.System.UserProfile;
using Floodlight.Client;

namespace Floodlight.Background
{
    public sealed class BackgroundChanger : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            var cachedBackgrounds = SettingsHelper.GetBackgroundCache();
            var bgIndex = new Random().Next(cachedBackgrounds.Count - 1);
            var background = cachedBackgrounds.Values.ElementAt(bgIndex);
            var backgroundFile = await FileHelper.GetBackgroundFromLocalFolder(background);

            await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(backgroundFile);
            await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(backgroundFile);
            deferral.Complete();
        }
    }
}

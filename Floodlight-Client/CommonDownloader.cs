using System;

namespace Floodlight.Client
{
    public static class CommonDownloader
    {
        public static async void Execute()
        {
            (await ServiceClient.GetUserBackgrounds()).ForEach(async background =>
            {
                FileManager.SaveBackgroundToLocalFolder(background, await ServiceClient.GetBackgroundImageStream(background.Id));
            });

            SettingsManager.Internal.LastRetrievedDate = DateTime.Now;
        }
    }
}

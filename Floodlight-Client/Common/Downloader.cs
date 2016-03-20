using System;
using System.Threading.Tasks;
using Floodlight.Client.Managers;

namespace Floodlight.Client.Common
{
    public static class Downloader
    {
        public static async Task Execute()
        {
            try
            {
                (await ServiceClient.GetUserBackgrounds()).ForEach(async background =>
                {
                    await FileManager.SaveBackgroundToLocalFolder(background,
                        await ServiceClient.GetBackgroundImageStream(background.Id));
                });

                SettingsManager.Internal.LastRetrievedDate = DateTime.Now;
            }
            catch
            {
                TelemetryManager.TrackEvent("Could not download metadata cache!");
            }
        }
    }
}

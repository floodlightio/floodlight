using System;
using System.Threading.Tasks;
using Floodlight.Client.Managers;

namespace Floodlight.Client.Common
{
    /// <summary>
    /// Provides common functionality around downloading the background metadata from the service.
    /// </summary>
    public static class Downloader
    {
        /// <summary>
        /// Run a single iteration of the downloading cycle:
        ///  - Contact the service to retrieve the latest background metadata
        ///  - Add any new background metadata to the local cache
        ///  - For any newly-added metadata, download the image and save it
        /// </summary>
        public static async Task Execute(bool onlyNew = true)
        {
            try
            {
                var userBackgrounds = await ServiceClient.GetUserBackgrounds(onlyNew);

                foreach (var background in userBackgrounds)
                {
                    var stream = await ServiceClient.GetBackgroundImageStream(background.Id);
                    await FileManager.SaveBackgroundToLocalFolder(background, stream);
                }
                SettingsManager.Internal.LastRetrievedDate = DateTime.Now;
            }
            catch
            {
                TelemetryManager.TrackEvent("Could not download metadata cache!");
            }
        }
    }
}

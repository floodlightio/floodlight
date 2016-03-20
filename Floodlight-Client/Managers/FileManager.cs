using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Floodlight.Client.Models;

namespace Floodlight.Client.Managers
{
    /// <summary>
    /// Manages saving/loading the background files from disk.
    /// </summary>
    public static class FileManager
    {
        /// <summary>
        /// Save the background specified to the disk.
        /// </summary>
        /// <param name="bg">The metadata for the background.</param>
        /// <param name="imageStream">The Stream for this image.</param>
        /// <remarks>The bg must contain a ContentType in order for the method to determine its extension.</remarks>
        /// <remarks>ContentType must be one of:
        ///  - image/jpeg
        ///  - image/png
        ///  - image/bmp</remarks>
        public static async Task SaveBackgroundToLocalFolder(Background bg, Stream imageStream)
        {
            var fileName = GetFileNameFromBackground(bg);
            var file = await SettingsManager.SettingsContainers.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                TelemetryManager.TrackEvent("Saving background image to disk...", new Dictionary<string, string>()
                {
                    { "backgroundId", bg.Id }
                });

                var task = imageStream.CopyToAsync(stream.AsStreamForWrite());
                imageStream.Dispose();
            }
        }

        /// <summary>
        /// Gets the specified background from the disk.
        /// </summary>
        /// <param name="bg">The metadata for the background.</param>
        /// <returns>The StorageFile associated with the background.</returns>
        public static async Task<StorageFile> GetBackgroundFromLocalFolder(Background bg)
        {
            try
            {
                return await SettingsManager.SettingsContainers.LocalFolder.GetFileAsync(GetFileNameFromBackground(bg));
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// Convert from the given ContentType to an extension.
        /// </summary>
        /// <param name="contentType">Requested contentType.</param>
        /// <returns>An extension to use for the content type.</returns>
        private static string ConvertContentTypeToExtension(string contentType)
        {
            if (contentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return "jpg";
            }
            else if (contentType.Equals("image/png", StringComparison.OrdinalIgnoreCase))
            {
                return "png";
            }
            else if (contentType.Equals("image/bmp", StringComparison.OrdinalIgnoreCase))
            {
                return "bmp";
            }

            throw new UnknownContentTypeException();
        }

        /// <summary>
        /// Construct the file name from the background metadata.
        /// </summary>
        /// <param name="bg">The background metadata to use.</param>
        /// <returns>A filename in the form [Guid].[Extension]</returns>
        private static string GetFileNameFromBackground(Background bg)
        {
            return string.Format("{0}.{1}", bg.Id, ConvertContentTypeToExtension(bg.ContentType));
        }
    }
}

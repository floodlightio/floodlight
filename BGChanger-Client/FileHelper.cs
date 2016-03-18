﻿using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using BGChanger.Client.Models;

namespace BGChanger.Client
{
    public static class FileHelper
    {
        public static async void SaveBackgroundToLocalFolder(Background bg, Stream imageStream)
        {
            var fileName = GetFileNameFromBackground(bg);
            var file = await SettingsHelper.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await imageStream.CopyToAsync(stream.AsStreamForWrite());
                imageStream.Dispose();
            }
        }

        public static async Task<StorageFile> GetBackgroundFromLocalFolder(Background bg)
        {
            return await SettingsHelper.LocalFolder.GetFileAsync(GetFileNameFromBackground(bg));
        }

        public static string ConvertContentTypeToExtension(string contentType)
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

        private static string GetFileNameFromBackground(Background bg)
        {
            return string.Format("{0}.{1}", bg.Id, ConvertContentTypeToExtension(bg.ContentType));
        }
    }
}
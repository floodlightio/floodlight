﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.Data.Json;
using Windows.Storage;
using BGChanger.Client.Models;

namespace BGChanger.Client
{
    public static class SettingsHelper
    {
        public static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        public static StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;

        public static string ServerAddress
        {
            get
            {
                if (!LocalSettings.Values.ContainsKey("URL"))
                {
                    LocalSettings.Values["URL"] = "http://bg.zaharia.io/";
                }

                // For testing purposes only
                LocalSettings.Values["URL"] = "http://bgchanger-dev.azurewebsites.net/";

                return LocalSettings.Values["URL"].ToString();
            }
        }

        public static string UserId
        {
            get
            {
                if (!LocalSettings.Values.ContainsKey("UserId"))
                {
                    // For testing purposes only
                    Guid rawGuid = Guid.NewGuid();
                    String base64Guid = Convert.ToBase64String(rawGuid.ToByteArray())
                                               .Substring(0, 22)
                                               .Replace("/", "_")
                                               .Replace("+", "-");

                    LocalSettings.Values["UserId"] = base64Guid;
                }

                return LocalSettings.Values["UserId"].ToString();
            }
        }

        public static readonly string ApiEndpoint = "api/{0}";
        public static string UserDetailsEndpoint = string.Format(ApiEndpoint, "user/{0}");
        public static readonly string UserBackgroundsEndpoint = string.Format(ApiEndpoint, "user/{0}/backgrounds");
        public static readonly string BackgroundDetailsEndpoint = string.Format(ApiEndpoint, "background/{0}");
        public static readonly string BackgroundImageEndpoint = string.Format(ApiEndpoint, "background/{0}/image");

        public static DateTime LastUpdatedDate
        {
            get
            {
                if (!LocalSettings.Values.ContainsKey("LastUpdatedDate"))
                {
                    LocalSettings.Values["LastUpdatedDate"] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
                }

                return DateTime.Parse((string)LocalSettings.Values["LastUpdatedDate"]);
            }

            set { LocalSettings.Values["LastUpdatedDate"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public static bool UpdateWallpaper
        {
            get
            {
                if (!LocalSettings.Values.ContainsKey("UpdateWallpaper"))
                {
                    LocalSettings.Values["UpdateWallpaper"] = false;
                }

                return (bool)LocalSettings.Values["UpdateWallpaper"];
            }

            set { LocalSettings.Values["UpdateWallpaper"] = value; }
        }

        public static bool UpdateLockScreen
        {
            get
            {
                if (!LocalSettings.Values.ContainsKey("UpdateLockScreen"))
                {
                    LocalSettings.Values["UpdateLockScreen"] = false;
                }

                return (bool)LocalSettings.Values["UpdateLockScreen"];
            }

            set { LocalSettings.Values["UpdateLockScreen"] = value; }
        }

        public static Dictionary<string, Background> GetBackgroundCache()
        {
            if (LocalSettings.Values.ContainsKey("BackgroundCache"))
            {
                var composite = (ApplicationDataCompositeValue)LocalSettings.Values["BackgroundCache"];

                if (composite != null)
                {
                    return composite.Values
                        .Select(o =>
                        {
                            var json = (string)o;

                            var jsonObject = JsonObject.Parse(json);
                            return ServiceClient.DeserializeBackground(jsonObject);
                        })
                        .ToDictionary(background => background.Id);
                }
            }

            LocalSettings.Values["BackgroundCache"] = new ApplicationDataCompositeValue();

            return new Dictionary<string, Background>();
        }

        public static List<Background> AddToBackgroundCache(List<Background> backgrounds)
        {
            var cachedBackgrounds = GetBackgroundCache();
            var uncachedBackgrounds = backgrounds.Where(bg => !cachedBackgrounds.ContainsKey(bg.Id)).ToList();
            var cache = (ApplicationDataCompositeValue)LocalSettings.Values["BackgroundCache"];

            foreach (var bg in uncachedBackgrounds)
            {
                cache.Add(bg.Id, ServiceClient.SerializeBackground(bg).Stringify());
            }

            LocalSettings.Values["BackgroundCache"] = cache;

            return uncachedBackgrounds;
        }

        public static void ClearBackgroundCache()
        {
            LocalSettings.Values.Remove("BackgroundCache");
        }
    }
}
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.Data.Json;
using Windows.Storage;
using Floodlight.Client.Models;

namespace Floodlight.Client.Managers
{
    public static class SettingsManager
    {
        /**
         * User-Changeable Settings
         */
        public static class UserDefined
        {
            public static string ServiceAddress
            {
                get
                {
                    if (!LocalSettings.Values.ContainsKey("ServiceAddress"))
                    {
                        // Default Service URL for Floodlight service
                        LocalSettings.Values["ServiceAddress"] = "http://floodlight.io/";
                    }

                    // TODO: For Testing Purposes Only, Remove
                    LocalSettings.Values["ServiceAddress"] = "http://dev.floodlight.io/";

                    return LocalSettings.Values["ServiceAddress"].ToString();
                }

                set { LocalSettings.Values["ServiceAddress"] = value; }
            }

            public static string UserId
            {
                get
                {
                    if (!LocalSettings.Values.ContainsKey("UserId"))
                    {
                        // TODO: For Testing Purposes Only, Remove
                        Guid rawGuid = Guid.NewGuid();
                        String base64Guid = Convert.ToBase64String(rawGuid.ToByteArray())
                            .Substring(0, 22)
                            .Replace("/", "_")
                            .Replace("+", "-");

                        LocalSettings.Values["UserId"] = base64Guid;
                    }

                    return LocalSettings.Values["UserId"].ToString();
                }

                set { LocalSettings.Values["UserId"] = value; }
            }

            public static bool UpdateWallpaper
            {
                get
                {
                    if (!LocalSettings.Values.ContainsKey("UpdateWallpaper"))
                    {
                        LocalSettings.Values["UpdateWallpaper"] = false;
                    }

                    return (bool) LocalSettings.Values["UpdateWallpaper"];
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

                    return (bool) LocalSettings.Values["UpdateLockScreen"];
                }

                set { LocalSettings.Values["UpdateLockScreen"] = value; }
            }

            public static bool UseSameImage
            {
                get
                {
                    if (!LocalSettings.Values.ContainsKey("UseSameImage"))
                    {
                        LocalSettings.Values["UseSameImage"] = false;
                    }

                    return (bool)LocalSettings.Values["UseSameImage"];
                }

                set { LocalSettings.Values["UseSameImage"] = value; }
            }
        }

        /**
         * Internal Settings
         */
        public static class Internal
        {
            public static DateTime LastRetrievedDate
            {
                get
                {
                    return !LocalSettings.Values.ContainsKey("LastRetrievedDate") ? DateTime.MinValue : DateTime.Parse((string) LocalSettings.Values["LastRetrievedDate"]);
                }

                set { LocalSettings.Values["LastRetrievedDate"] = value.ToString(CultureInfo.InvariantCulture); }
            }

            public static DateTime LastUpdatedDate
            {
                get
                {
                    return !LocalSettings.Values.ContainsKey("LastUpdatedDate") ? DateTime.MinValue : DateTime.Parse((string)LocalSettings.Values["LastUpdatedDate"]);
                }

                set { LocalSettings.Values["LastUpdatedDate"] = value.ToString(CultureInfo.InvariantCulture); }
            }

            public static Background CurrentWallpaper
            {
                get
                {
                    if (!LocalSettings.Values.ContainsKey("CurrentWallpaper"))
                    {
                        return null;
                    }

                    var jsonObject = JsonObject.Parse((string)LocalSettings.Values["CurrentWallpaper"]);
                    return ServiceClient.DeserializeBackground(jsonObject);
                }

                set { LocalSettings.Values["CurrentWallpaper"] = ServiceClient.SerializeBackground(value).Stringify(); }
            }

            public static Background CurrentLockScreen
            {
                get
                {
                    if (!LocalSettings.Values.ContainsKey("CurrentLockScreen"))
                    {
                        return null;
                    }

                    var jsonObject = JsonObject.Parse((string)LocalSettings.Values["CurrentLockScreen"]);
                    return ServiceClient.DeserializeBackground(jsonObject);
                }

                set { LocalSettings.Values["CurrentLockScreen"] = ServiceClient.SerializeBackground(value).Stringify(); }
            }

            public static Dictionary<string, Background> GetBackgroundCache()
            {
                if (LocalSettings.Values.ContainsKey("BackgroundCache"))
                {
                    var composite = (ApplicationDataCompositeValue) LocalSettings.Values["BackgroundCache"];

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
                var cache = (ApplicationDataCompositeValue) LocalSettings.Values["BackgroundCache"];

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

        /**
         * UWP Settings Containers
         */
        #region UWP Settings
        public static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        public static readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;
        #endregion

        /**
         * Service API Endpoints
         */
        #region API Endpoints
        public const string ApiEndpoint = "api/{0}";
        public static string UserDetailsEndpoint = string.Format(ApiEndpoint, "user/{0}");
        public static readonly string UserBackgroundsEndpoint = string.Format(ApiEndpoint, "user/{0}/backgrounds");
        public static readonly string BackgroundDetailsEndpoint = string.Format(ApiEndpoint, "background/{0}");
        public static readonly string BackgroundImageEndpoint = string.Format(ApiEndpoint, "background/{0}/image");
        #endregion
    }
}

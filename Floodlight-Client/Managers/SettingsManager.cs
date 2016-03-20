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
    /// <summary>
    /// Manages saving/loading data from app settings.
    /// </summary>
    public static class SettingsManager
    {
        /// <summary>
        /// User-Defined Settings
        /// </summary>
        public static class UserDefined
        {
            /// <summary>
            /// The service address to connect to.
            /// By default, this is https://floodlight.io
            /// </summary>
            public static string ServiceAddress
            {
                get
                {
                    if (!LocalSettings.Values.ContainsKey("ServiceAddress"))
                    {
                        // Default Service URL for Floodlight service
                        LocalSettings.Values["ServiceAddress"] = "https://floodlight.io/";
                    }

                    // TODO: For Testing Purposes Only, Remove
                    LocalSettings.Values["ServiceAddress"] = "https://dev.floodlight.io/";

                    return LocalSettings.Values["ServiceAddress"].ToString();
                }

                set { LocalSettings.Values["ServiceAddress"] = value; }
            }

            /// <summary>
            /// The ID of the user to query backgrounds for.
            /// </summary>
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

            /// <summary>
            /// Whether the wallpaper should be updated.
            /// </summary>
            public static bool UpdateWallpaper
            {
                get
                {
                    if (!LocalSettings.Values.ContainsKey("UpdateWallpaper"))
                    {
                        LocalSettings.Values["UpdateWallpaper"] = true;
                    }

                    return (bool) LocalSettings.Values["UpdateWallpaper"];
                }

                set { LocalSettings.Values["UpdateWallpaper"] = value; }
            }

            /// <summary>
            /// Whether the lock screen should be updated.
            /// </summary>
            public static bool UpdateLockScreen
            {
                get
                {
                    if (!LocalSettings.Values.ContainsKey("UpdateLockScreen"))
                    {
                        LocalSettings.Values["UpdateLockScreen"] = true;
                    }

                    return (bool) LocalSettings.Values["UpdateLockScreen"];
                }

                set { LocalSettings.Values["UpdateLockScreen"] = value; }
            }

            /// <summary>
            /// Whether or not to use the same image for both wallpaper and lock screen updates.
            /// </summary>
            public static bool UseSameImage
            {
                get
                {
                    if (!LocalSettings.Values.ContainsKey("UseSameImage"))
                    {
                        LocalSettings.Values["UseSameImage"] = true;
                    }

                    return (bool)LocalSettings.Values["UseSameImage"];
                }

                set { LocalSettings.Values["UseSameImage"] = value; }
            }
        }

        /// <summary>
        /// Internal app state
        /// </summary>
        /// <remarks>These settings should not be user-changeable, they represent the internal state.</remarks>
        public static class Internal
        {
            /// <summary>
            /// The date the background metadata was last received.
            /// </summary>
            public static DateTime LastRetrievedDate
            {
                get
                {
                    return !LocalSettings.Values.ContainsKey("LastRetrievedDate") ? DateTime.MinValue : DateTime.Parse((string) LocalSettings.Values["LastRetrievedDate"]);
                }

                set { LocalSettings.Values["LastRetrievedDate"] = value.ToString(CultureInfo.InvariantCulture); }
            }

            /// <summary>
            /// The date the wallpaper and/or lock screen was last updated.
            /// </summary>
            public static DateTime LastUpdatedDate
            {
                get
                {
                    return !LocalSettings.Values.ContainsKey("LastUpdatedDate") ? DateTime.MinValue : DateTime.Parse((string)LocalSettings.Values["LastUpdatedDate"]);
                }

                set { LocalSettings.Values["LastUpdatedDate"] = value.ToString(CultureInfo.InvariantCulture); }
            }

            /// <summary>
            /// The title of the current wallpaper used.
            /// </summary>
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

            /// <summary>
            /// The title of the current lock screen used.
            /// </summary>
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

            /// <summary>
            /// Get the local background metadata cache from app settings.
            /// </summary>
            /// <returns>The local copy of background metadata available.</returns>
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

            /// <summary>
            /// Add a list of backgrounds to the local cache.
            /// </summary>
            /// <param name="backgrounds">The backgrounds to add</param>
            /// <returns>The list of backgrounds that did not already exist in the cache</returns>
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

            /// <summary>
            /// Clear out the locak background cache.
            /// </summary>
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

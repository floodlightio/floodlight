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
                    if (!SettingsContainers.LocalSettings.Values.ContainsKey("ServiceAddress"))
                    {
                        // Default Service URL for Floodlight service
                        SettingsContainers.LocalSettings.Values["ServiceAddress"] = "https://floodlight.io/";
                    }

                    // TODO: For Testing Purposes Only, Remove
                    SettingsContainers.LocalSettings.Values["ServiceAddress"] = "https://dev.floodlight.io/";

                    return SettingsContainers.LocalSettings.Values["ServiceAddress"].ToString();
                }

                set { SettingsContainers.LocalSettings.Values["ServiceAddress"] = value; }
            }

            /// <summary>
            /// The ID of the user to query backgrounds for.
            /// </summary>
            public static string UserId
            {
                get
                {
                    if (!SettingsContainers.LocalSettings.Values.ContainsKey("UserId"))
                    {
                        // TODO: For Testing Purposes Only, Remove
                        Guid rawGuid = Guid.NewGuid();
                        string base64Guid = Convert.ToBase64String(rawGuid.ToByteArray())
                            .Substring(0, 22)
                            .Replace("/", "_")
                            .Replace("+", "-");

                        SettingsContainers.LocalSettings.Values["UserId"] = base64Guid;
                    }

                    return SettingsContainers.LocalSettings.Values["UserId"].ToString();
                }

                set { SettingsContainers.LocalSettings.Values["UserId"] = value; }
            }

            /// <summary>
            /// How often to run the background tasks to download metadata and update the images.
            /// </summary>
            public static int UpdateFrequency
            {
                get
                {
                    if (!SettingsContainers.LocalSettings.Values.ContainsKey("UpdateFrequency"))
                    {
                        SettingsContainers.LocalSettings.Values["UpdateFrequency"] = 15;
                    }

                    return (int)SettingsContainers.LocalSettings.Values["UpdateFrequency"];
                }

                set { SettingsContainers.LocalSettings.Values["UpdateFrequency"] = value; }
            }

            /// <summary>
            /// Whether the wallpaper should be updated.
            /// </summary>
            public static bool UpdateWallpaper
            {
                get
                {
                    if (!SettingsContainers.LocalSettings.Values.ContainsKey("UpdateWallpaper"))
                    {
                        SettingsContainers.LocalSettings.Values["UpdateWallpaper"] = true;
                    }

                    return (bool) SettingsContainers.LocalSettings.Values["UpdateWallpaper"];
                }

                set { SettingsContainers.LocalSettings.Values["UpdateWallpaper"] = value; }
            }

            /// <summary>
            /// Whether the lock screen should be updated.
            /// </summary>
            public static bool UpdateLockScreen
            {
                get
                {
                    if (!SettingsContainers.LocalSettings.Values.ContainsKey("UpdateLockScreen"))
                    {
                        SettingsContainers.LocalSettings.Values["UpdateLockScreen"] = true;
                    }

                    return (bool) SettingsContainers.LocalSettings.Values["UpdateLockScreen"];
                }

                set { SettingsContainers.LocalSettings.Values["UpdateLockScreen"] = value; }
            }

            /// <summary>
            /// Whether or not to use the same image for both wallpaper and lock screen updates.
            /// </summary>
            public static bool UseSameImage
            {
                get
                {
                    if (!SettingsContainers.LocalSettings.Values.ContainsKey("UseSameImage"))
                    {
                        SettingsContainers.LocalSettings.Values["UseSameImage"] = true;
                    }

                    return (bool) SettingsContainers.LocalSettings.Values["UseSameImage"];
                }

                set { SettingsContainers.LocalSettings.Values["UseSameImage"] = value; }
            }
        }

        /// <summary>
        /// Internal app state
        /// </summary>
        /// <remarks>These settings should not be user-changeable, they represent the internal state.</remarks>
        public static class Internal
        {
            /// <summary>
            /// The date the background metadata was last retrieved from the Floodlight service.
            /// </summary>
            public static DateTime LastRetrievedDate
            {
                get
                {
                    return !SettingsContainers.LocalSettings.Values.ContainsKey("LastRetrievedDate") ? DateTime.MinValue : DateTime.Parse((string) SettingsContainers.LocalSettings.Values["LastRetrievedDate"]);
                }

                set { SettingsContainers.LocalSettings.Values["LastRetrievedDate"] = value.ToString(CultureInfo.InvariantCulture); }
            }

            /// <summary>
            /// The date the wallpaper and/or lock screen was last updated.
            /// </summary>
            public static DateTime LastUpdatedDate
            {
                get
                {
                    return !SettingsContainers.LocalSettings.Values.ContainsKey("LastUpdatedDate") ? DateTime.MinValue : DateTime.Parse((string) SettingsContainers.LocalSettings.Values["LastUpdatedDate"]);
                }

                set { SettingsContainers.LocalSettings.Values["LastUpdatedDate"] = value.ToString(CultureInfo.InvariantCulture); }
            }

            /// <summary>
            /// The title of the current wallpaper used.
            /// </summary>
            public static Background CurrentWallpaper
            {
                get
                {
                    if (!SettingsContainers.LocalSettings.Values.ContainsKey("CurrentWallpaper"))
                    {
                        return null;
                    }

                    var jsonObject = JsonObject.Parse((string) SettingsContainers.LocalSettings.Values["CurrentWallpaper"]);
                    return ServiceClient.DeserializeBackground(jsonObject);
                }

                set { SettingsContainers.LocalSettings.Values["CurrentWallpaper"] = ServiceClient.SerializeBackground(value).Stringify(); }
            }

            /// <summary>
            /// The title of the current lock screen used.
            /// </summary>
            public static Background CurrentLockScreen
            {
                get
                {
                    if (!SettingsContainers.LocalSettings.Values.ContainsKey("CurrentLockScreen"))
                    {
                        return null;
                    }

                    var jsonObject = JsonObject.Parse((string) SettingsContainers.LocalSettings.Values["CurrentLockScreen"]);
                    return ServiceClient.DeserializeBackground(jsonObject);
                }

                set { SettingsContainers.LocalSettings.Values["CurrentLockScreen"] = ServiceClient.SerializeBackground(value).Stringify(); }
            }

            /// <summary>
            /// Get the local background metadata cache from app settings.
            /// </summary>
            /// <returns>The local copy of background metadata available.</returns>
            public static Dictionary<string, Background> GetBackgroundCache()
            {
                if (SettingsContainers.LocalSettings.Values.ContainsKey("BackgroundCache"))
                {
                    var composite = (ApplicationDataCompositeValue) SettingsContainers.LocalSettings.Values["BackgroundCache"];

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

                SettingsContainers.LocalSettings.Values["BackgroundCache"] = new ApplicationDataCompositeValue();

                return new Dictionary<string, Background>();
            }

            /// <summary>
            /// Add a list of background metadatas to the local cache.
            /// </summary>
            /// <param name="backgrounds">The background metadatas to add</param>
            /// <returns>The list of background metadatas that did not already exist in the cache</returns>
            public static List<Background> AddToBackgroundCache(List<Background> backgrounds)
            {
                var cachedBackgrounds = GetBackgroundCache();
                var uncachedBackgrounds = backgrounds.Where(bg => !cachedBackgrounds.ContainsKey(bg.Id)).ToList();
                var cache = (ApplicationDataCompositeValue) SettingsContainers.LocalSettings.Values["BackgroundCache"];

                foreach (var bg in uncachedBackgrounds)
                {
                    cache.Add(bg.Id, ServiceClient.SerializeBackground(bg).Stringify());
                }

                SettingsContainers.LocalSettings.Values["BackgroundCache"] = cache;

                return uncachedBackgrounds;
            }

            /// <summary>
            /// Clear out the local background metadata cache.
            /// </summary>
            public static void ClearBackgroundCache()
            {
                SettingsContainers.LocalSettings.Values.Remove("BackgroundCache");
            }
        }

        /// <summary>
        /// UWP Setting Containers
        /// </summary>

        public static class SettingsContainers
        {
            public static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
            public static readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;
        }

        /// <summary>
        /// API Endpoints
        /// </summary>
        public static class ApiEndpoints
        {
            public const string ApiEndpoint = "api/{0}";
            public static string UserDetailsEndpoint = string.Format(ApiEndpoint, "user/{0}");
            public static readonly string UserBackgroundsEndpoint = string.Format(ApiEndpoint, "user/{0}/backgrounds");
            public static readonly string BackgroundDetailsEndpoint = string.Format(ApiEndpoint, "background/{0}");
            public static readonly string BackgroundImageEndpoint = string.Format(ApiEndpoint, "background/{0}/image");
        }
    }
}

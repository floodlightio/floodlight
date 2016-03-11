using System;

namespace BGChanger.Client
{
    internal static class SettingsHelper
    {
        public static Windows.Storage.ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public static string Url
        {
            get
            {
                if (!LocalSettings.Values.ContainsKey("URL"))
                {
                    LocalSettings.Values["URL"] = "http://bg.zaharia.io/";
                }

                return LocalSettings.Values["URL"].ToString();
            }
        }

        public static string ApiKey
        {
            get
            {
                if (!LocalSettings.Values.ContainsKey("APIKey"))
                {
                    Guid rawGuid = Guid.NewGuid();
                    String base64Guid = Convert.ToBase64String(rawGuid.ToByteArray())
                                               .Substring(0, 22)
                                               .Replace("/", "_")
                                               .Replace("+", "-");

                    LocalSettings.Values["APIKey"] = base64Guid;
                }

                return LocalSettings.Values["APIKey"].ToString();
            }
        }

        public static DateTime LastUpdatedDate => DateTime.UtcNow;

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
    }
}

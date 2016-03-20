using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;
using Floodlight.Client.Managers;
using Floodlight.Client.Models;

namespace Floodlight.Client
{
    /// <summary>
    /// The client that is responsible for connecting to the Floodlight service.
    /// </summary>
    public static class ServiceClient
    {
        /// <summary>
        /// Get details about the currently logged in user.
        /// </summary>
        public static void GetUserDetails()
        {
            // TODO: Do stuff here. Not sure what.   
        }

        /// <summary>
        /// Get a user's backgrounds.
        /// </summary>
        /// <param name="onlyNew">Whether to return only new backgrounds or all.</param>
        /// <returns>If onlyNew is set to true, returns only backgrounds not found in the local cache. Otherwise, all retrieved.</returns>
        public static async Task<List<Background>> GetUserBackgrounds(bool onlyNew = true)
        {
            var url = SettingsManager.UserDefined.ServiceAddress +
                      string.Format(SettingsManager.UserBackgroundsEndpoint, SettingsManager.UserDefined.UserId);

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var backgrounds = JsonArray.Parse(jsonResponse)
                            .Select(o => DeserializeBackground(o.GetObject()))
                            .ToList();

                    var addedBackgrounds = SettingsManager.Internal.AddToBackgroundCache(backgrounds);

                    TelemetryManager.TrackEvent("Downloaded background metadata from server",
                        new Dictionary<string, string>()
                        {
                            { "downloadedBackgrounds", backgrounds.Count().ToString() },
                            { "addedBackgrounds", addedBackgrounds.Count().ToString() }
                        });

                    return onlyNew ? addedBackgrounds : backgrounds;
                }
            }
        }

        /// <summary>
        /// Get details of a particular background by ID.
        /// </summary>
        /// <param name="backgroundId">The ID of the background to get details for.</param>
        /// <returns>The metadata requested.</returns>
        public static async Task<Background> GetBackgroundDetails(string backgroundId)
        {
            var url = SettingsManager.UserDefined.ServiceAddress +
                      string.Format(SettingsManager.BackgroundDetailsEndpoint, backgroundId);

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonObject = JsonObject.Parse(jsonResponse);

                    return DeserializeBackground(jsonObject);
                }
            }
        }

        /// <summary>
        /// Get the background image from the service.
        /// </summary>
        /// <param name="backgroundId">The ID of the background to get.</param>
        /// <returns>A Stream of the image retrieved from the server.</returns>
        public static async Task<Stream> GetBackgroundImageStream(string backgroundId)
        {
            var url = SettingsManager.UserDefined.ServiceAddress +
                      string.Format(SettingsManager.BackgroundImageEndpoint, backgroundId);
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }

        /// <summary>
        /// Serialize the background.
        /// </summary>
        /// <param name="bg">The background to serialise.</param>
        /// <returns>A JsonObject representing the Background.</returns>
        public static JsonObject SerializeBackground(Background bg)
        {
            var jsonObject = new JsonObject
            {
                { "Id", JsonValue.CreateStringValue(bg.Id) },
                { "Title", JsonValue.CreateStringValue(bg.Title) },
                { "ContentType", JsonValue.CreateStringValue(bg.ContentType) }
            };

            return jsonObject;
        }

        /// <summary>
        /// Deserialize the Background.
        /// </summary>
        /// <param name="jsonObject">A JsonObject representing the background.</param>
        /// <returns>The deserialized Background object.</returns>
        public static Background DeserializeBackground(JsonObject jsonObject)
        {
            return new Background()
            {
                Id = jsonObject.GetNamedString("Id"),
                Title = jsonObject.GetNamedString("Title"),
                ContentType = jsonObject.GetNamedString("ContentType")
            };
        }
    }

    public class UnknownContentTypeException : Exception { }
}

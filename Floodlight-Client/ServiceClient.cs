using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;
using Floodlight.Client.Models;

namespace Floodlight.Client
{
    public static class ServiceClient
    {
        public static void GetUserDetails()
        {
            // TODO: Do stuff here. Not sure what.   
        }

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

                    return onlyNew ? addedBackgrounds : backgrounds;
                }
            }
        }

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

        public static async Task<Stream> GetBackgroundImageStream(string backgroundId)
        {
            var url = SettingsManager.UserDefined.ServiceAddress +
                      string.Format(SettingsManager.BackgroundImageEndpoint, backgroundId);
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }

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

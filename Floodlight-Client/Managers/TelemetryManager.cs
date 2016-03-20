using System.Collections.Generic;
using HockeyApp;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Floodlight.Client.Managers
{
    /// <summary>
    /// Manages telemetry for the app.
    /// </summary>
    public static class TelemetryManager
    {
        private static TelemetryClient Client { get; } = new TelemetryClient();

        private const string AppInsightsIKey = "c1f11a8a-5efe-45bc-a063-aafd5e3935ce";
        private const string HockeyAppKey = "ad1f37a8bc9f41a182adaf006191b398";

        /// <summary>
        /// Initialize full AI and HA telemetry for the app.
        /// </summary>
        public static void InitializeAppTelemetry()
        {
            WindowsAppInitializer.InitializeAsync(
                AppInsightsIKey,
                WindowsCollectors.Metadata |
                WindowsCollectors.Session |
                WindowsCollectors.UnhandledException);
            HockeyClient.Current.Configure(HockeyAppKey);
        }

        /// <summary>
        /// Initialize a lighter AI-only telemetry for background tasks.
        /// </summary>
        public static void InitializeBackgroundTelemetry()
        {
            TelemetryConfiguration.Active.InstrumentationKey = AppInsightsIKey;
        }

        /// <summary>
        /// Track an event.
        /// </summary>
        /// <param name="eventText">The text of the event.</param>
        /// <param name="properties">Any properties to append to the event.</param>
        public static void TrackEvent(string eventText, Dictionary<string, string> properties = null)
        {
            Client.TrackEvent(eventText, properties);
            Client.Flush();
        }
    }
}

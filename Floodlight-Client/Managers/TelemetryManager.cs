using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HockeyApp;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Floodlight.Client.Managers
{
    public static class TelemetryManager
    {
        private static TelemetryClient Client { get; } = new TelemetryClient();

        private const string AppInsightsIKey = "c1f11a8a-5efe-45bc-a063-aafd5e3935ce";
        private const string HockeyAppKey = "ad1f37a8bc9f41a182adaf006191b398";

        public static void InitializeAppTelemetry()
        {
            WindowsAppInitializer.InitializeAsync(
                AppInsightsIKey,
                WindowsCollectors.Metadata |
                WindowsCollectors.Session |
                WindowsCollectors.UnhandledException);
            HockeyClient.Current.Configure(HockeyAppKey);
        }

        public static void InitializeBackgroundTelemetry()
        {
            TelemetryConfiguration.Active.InstrumentationKey = AppInsightsIKey;
        }

        public static void TrackEvent(string eventText, Dictionary<string, string> properties = null)
        {
            Client.TrackEvent(eventText, properties);
            Client.Flush();
        }
    }
}

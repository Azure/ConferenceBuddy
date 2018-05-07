using System;
using System.Configuration;

namespace Microsoft.ConferenceBuddy.BotBrain.Utilities
{
    /// <summary>
    /// Utility class to extract application settings
    /// </summary>
    public static class SettingsUtility
    {
        // Azure Search
        public static string AzureSearchServiceName => ConfigurationManager.AppSettings["AzureSearchServiceName"]?.ToString();

        public static string AzureSearchServiceKey => ConfigurationManager.AppSettings["AzureSearchServiceKey"]?.ToString();

        public static string AzureSearchIndexName => ConfigurationManager.AppSettings["AzureSearchIndexName"]?.ToString();

        // Bing Search
        public static string BingSearchUrl => ConfigurationManager.AppSettings["BingSearchUrl"]?.ToString();

        public static string BingSearchSubscriptionKey => ConfigurationManager.AppSettings["BingSearchSubscriptionKey"]?.ToString();

        // Default max results count
        public static int MaxResultsCount => Int32.Parse(ConfigurationManager.AppSettings["MaxResultsCount"]?.ToString());

        // QnA Maker
        public static string QnAMakerUrl => ConfigurationManager.AppSettings["QnAMakerUrl"]?.ToString();

        public static string QnAMakerSubscriptionKey => ConfigurationManager.AppSettings["QnAMakerSubscriptionKey"]?.ToString();

        public static string QnAMakerWhoTaskKnowledgebaseId => ConfigurationManager.AppSettings["QnAMakerWhoTaskKnowledgebaseId"]?.ToString();

        // Text Analytics
        public static string TextAnalyticsUrl => ConfigurationManager.AppSettings["TextAnalyticsUrl"]?.ToString();

        public static string TextAnalyticsSubscriptionKey => ConfigurationManager.AppSettings["TextAnalyticsSubscriptionKey"]?.ToString();

        // Translator
        public static string TranslatorUrl => ConfigurationManager.AppSettings["TranslatorUrl"]?.ToString();

        public static string TranslatorSubscriptionKey => ConfigurationManager.AppSettings["TranslatorSubscriptionKey"]?.ToString();

        // Video Indexer
        public static string VideoIndexerUrl => ConfigurationManager.AppSettings["VideoIndexerUrl"]?.ToString();

        public static string VideoIndexerSubscriptionKey => ConfigurationManager.AppSettings["VideoIndexerSubscriptionKey"]?.ToString();
    }
}

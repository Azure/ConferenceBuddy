using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

using Microsoft.ConferenceBuddy.BotBrain.Models;
using Microsoft.ConferenceBuddy.Common.Models;
using Microsoft.ConferenceBuddy.Core.Services;

namespace Microsoft.ConferenceBuddy.BotBrain.Utilities
{
    /// <summary>
    /// Utility class to for services
    /// </summary>
    public static class ServicesUtility
    {
        #region Services

        private static BingSearchService bingSearch;

        public static BingSearchService BingSearch
        {
            get
            {
                if (bingSearch == null)
                {
                    bingSearch = new BingSearchService(
                        serviceUrl: SettingsUtility.BingSearchUrl,
                        subscriptionKey: SettingsUtility.BingSearchSubscriptionKey
                        );
                }
                return bingSearch;
            }
        }

        private static QnAMakerService qnaMaker;

        public static QnAMakerService QnAMaker
        {
            get
            {
                if (qnaMaker == null)
                {
                    qnaMaker = new QnAMakerService(
                        serviceUrl: SettingsUtility.QnAMakerUrl,
                        subscriptionKey: SettingsUtility.QnAMakerSubscriptionKey
                        );
                }
                return qnaMaker;
            }
        }

        private static SearchServiceClient searchService;

        public static SearchServiceClient SearchService
        {
            get
            {
                if (searchService == null)
                {
                    searchService =
                        new SearchServiceClient(SettingsUtility.AzureSearchServiceName, new SearchCredentials(SettingsUtility.AzureSearchServiceKey));
                }
                return searchService;
            }
        }

        private static TextAnalyticsService textAnalytics;

        public static TextAnalyticsService TextAnalytics
        {
            get
            {
                if (textAnalytics == null)
                {
                    textAnalytics = new TextAnalyticsService(
                        serviceUrl: SettingsUtility.TextAnalyticsUrl,
                        subscriptionKey: SettingsUtility.TextAnalyticsSubscriptionKey);
                }
                return textAnalytics;
            }
        }

        private static VideoIndexerService videoIndexer;

        public static VideoIndexerService VideoIndexer
        {
            get
            {
                if (videoIndexer == null)
                {
                    videoIndexer = new VideoIndexerService(
                        serviceUrl: SettingsUtility.VideoIndexerUrl,
                        subscriptionKey: SettingsUtility.VideoIndexerSubscriptionKey);
                }
                return videoIndexer;
            }
        }

        #endregion

        /// <summary>
        /// Get Ask Question Results from Bing Web Search Result
        /// </summary>
        public static async Task<string[]> GetTopics(string question, string[] currentTopics)
        {
            // Run keyphrases extraction
            TextAnalyticsResult<TextAnalyticsKeyPhrasesResult> textAnalyticsResult =
                await TextAnalytics.AnalyzeKeyPhrasesAsync(question);

            string[] topics = currentTopics;

            if (textAnalyticsResult.Results?.Count() > 0 == true)
            {
                topics = textAnalyticsResult.Results.Select(r => r.KeyPhrases).FirstOrDefault();
            }

            if (topics?.Count() > 0)
            {
                topics = topics.Select(t => t.ToLower()).ToArray();
            }

            return topics;
        }

        /// <summary>
        /// Get Ask Question Results from Bing Web Search Result
        /// </summary>
        public static AskQuestionResult[] GetAskQuestionResults(BingWebSearchResult searchResult)
        {
            AskQuestionResult[] results = searchResult.WebPagesResult.Values
                .Take(SettingsUtility.MaxResultsCount)
                .Select(page => new AskQuestionResult()
                {
                    Title = page.Name,
                    Answer = page.Snippet,
                    Source = "Bing",
                    Url = page.Url,
                    UrlDisplayName = "Learn More...",
                    ImageUrl = page.Screenshot?.ThumbnailUrl
                }).ToArray();

            return results;
        }

        /// <summary>
        /// Get Ask Question Results from QnA Maker Search Result
        /// </summary>
        public static AskQuestionResult[] GetAskQuestionResults(QnAMakerSearchResult searchResult, double confidenceScore, bool isAnswerJson = true)
        {
            AskQuestionResult[] results = searchResult.Answers
                .Where(a => a.Score >= confidenceScore)
                .Take(SettingsUtility.MaxResultsCount)
                .Select(a => isAnswerJson == true ?
                    JsonConvert.DeserializeObject<AskQuestionResult>(WebUtility.HtmlDecode(a.Answer))
                    : new AskQuestionResult() { Answer = WebUtility.HtmlDecode(a.Answer) })
                .ToArray();

            return results;
        }

        /// <summary>
        /// Get Ask Question Results from Video Indexer Search Result
        /// </summary>
        public static AskQuestionResult[] GetAskQuestionResults(VideoIndexerSearchResult searchResult)
        {
            List<AskQuestionResult> results = new List<AskQuestionResult>();
            foreach (VideoIndexerResult videoResult in searchResult.Results.Take(SettingsUtility.MaxResultsCount))
            {
                AskQuestionResult result = new AskQuestionResult()
                {
                    Title = videoResult.Name,
                    ImageUrl = videoResult.ThumbnailUrl,
                    Source = "Video Indexer",
                    Url = videoResult.VideoUrl,
                    UrlDisplayName = "Open Video"
                };

                if (videoResult.SearchMatches?.Count() > 0 == true)
                {
                    VideoIndexerSearchMatch match =
                        videoResult.SearchMatches.FirstOrDefault(s => s.Type.Contains("Transcript") == true) ?? videoResult.SearchMatches.First();

                    result.Url += "?t=" + TimeSpan.Parse(match.StartTime).TotalSeconds.ToString();

                    result.Answer = match.text;
                }

                results.Add(result);
            }

            return results.ToArray();
        }
        
        /// <summary>
        /// Upload document to Azure search service
        /// </summary>
        public static async Task UploadDocumentToSearchService<TDocument>(string indexName, TDocument document)
            where TDocument : SearchDocument
        {
            ISearchIndexClient searchIndexClient = SearchService.Indexes.GetClient(indexName);

            IndexAction<TDocument>[] actions = new IndexAction<TDocument>[]
            {
                IndexAction.Upload(document)
            };

            IndexBatch<TDocument> batch = IndexBatch.New(actions);

            await searchIndexClient.Documents.IndexAsync(batch);
        }
    }
}

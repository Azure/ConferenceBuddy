using System;
using System.Runtime.Serialization;

namespace Microsoft.ConferenceBuddy.Common.Models
{
    [DataContract]
    public abstract class BingSearchResult
    {
        /// <summary>
        /// The type of result
        /// </summary>
        [DataMember(Name = "_type")]
        public string Type { get; set; }

        /// <summary>
        /// The query context of the search
        /// </summary>
        [DataMember(Name = "queryContext")]
        public BingQueryContext QueryContext { get; set; }
    }

    [DataContract]
    public class BingQueryContext
    {
        /// <summary>
        /// The original query
        /// </summary>
        [DataMember(Name = "originalQuery")]
        public string OriginalQuery { get; set; }
    }

    [DataContract]
    public class BingApisScreenshot
    {
        /// <summary>
        /// The webSearchUrl of screenshot
        /// </summary>
        [DataMember(Name = "webSearchUrl")]
        public string WebSearchUrl { get; set; }

        /// <summary>
        /// The thumbnailUrl of screenshot
        /// </summary>
        [DataMember(Name = "thumbnailUrl")]
        public string ThumbnailUrl { get; set; }
    }

    [DataContract]
    public class BingWebPagesResult : BingWebResult
    {
        /// <summary>
        /// The total estimated matches
        /// </summary>
        [DataMember(Name = "totalEstimatedMatches")]
        public int TotalEstimatedMatches { get; set; }

        /// <summary>
        /// The results value
        /// </summary>
        [DataMember(Name = "value")]
        public BingWebPagesResultValue[] Values { get; set; }
    }

    [DataContract]
    public class BingWebPagesResultValue
    {
        /// <summary>
        /// The id of the value
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// The screenshot of result
        /// </summary>
        [DataMember(Name = "screenshot")]
        public BingApisScreenshot Screenshot { get; set; }

        /// <summary>
        /// The name of the value
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The url of the value
        /// </summary>
        [DataMember(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// The snippet of the value
        /// </summary>
        [DataMember(Name = "snippet")]
        public string Snippet { get; set; }
    }

    [DataContract]
    public class BingWebResult
    {
        /// <summary>
        /// The web search url
        /// </summary>
        [DataMember(Name = "webSearchUrl")]
        public string WebSearchUrl { get; set; }
    }

    [DataContract]
    public class BingWebSearchResult : BingSearchResult
    {
        /// <summary>
        /// The web pages result
        /// </summary>
        [DataMember(Name = "webPages")]
        public BingWebPagesResult WebPagesResult { get; set; }
    }
}

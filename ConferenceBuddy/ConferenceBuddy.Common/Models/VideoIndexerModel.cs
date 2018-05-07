using System;
using System.Runtime.Serialization;

namespace Microsoft.ConferenceBuddy.Common.Models
{
    [DataContract]
    public class VideoIndexerSearchMatch
    {
        /// <summary>
        /// The start time of the Video Indexer search match
        /// </summary>
        [DataMember(Name = "startTime")]
        public string StartTime { get; set; }

        /// <summary>
        /// The type of the Video Indexer search match
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// The text of the Video Indexer search match
        /// </summary>
        [DataMember(Name = "text")]
        public string text { get; set; }
    }

    [DataContract]
    public class VideoIndexerResult
    {
        /// <summary>
        /// The id of the Video Indexer result
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the Video Indexer result
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The thumbnail url of the Video Indexer result
        /// </summary>
        [DataMember(Name = "thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// The search matches of the Video Indexer result
        /// </summary>
        [DataMember(Name = "searchMatches")]
        public VideoIndexerSearchMatch [] SearchMatches { get; set; }

        /// <summary>
        /// The video url of the Video Indexer result
        /// </summary>
        public string VideoUrl
        {
            get
            {
                return string.Format("https://www.videoindexer.ai/media/{0}", Id);
            }
        }
    }

    [DataContract]
    public class VideoIndexerSearchResult
    {
        /// <summary>
        /// The results of Video Indexer search
        /// </summary>
        [DataMember(Name = "results")]
        public VideoIndexerResult [] Results { get; set; }
    }
}

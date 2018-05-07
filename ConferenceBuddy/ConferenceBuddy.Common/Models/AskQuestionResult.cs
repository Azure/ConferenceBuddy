using System;
using System.Runtime.Serialization;

namespace Microsoft.ConferenceBuddy.Common.Models
{
    /// <summary>
    /// Data contract between Bot and Bot Brain to encapsulate the results within the response
    /// </summary>
    [DataContract]
    public class AskQuestionResult
    {
        /// <summary>
        /// The title of the result
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// The answer of the result
        /// </summary>
        [DataMember(Name = "answer")]
        public string Answer { get; set; }

        /// <summary>
        /// The image url of the result
        /// </summary>
        [DataMember(Name = "imageUrl")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// The source of the result
        /// </summary>
        [DataMember(Name = "source")]
        public string Source { get; set; }

        /// <summary>
        /// The url of the result
        /// </summary>
        [DataMember(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// The url display name of the result
        /// </summary>
        [DataMember(Name = "urlDisplayName")]
        public string UrlDisplayName { get; set; }
    }
}

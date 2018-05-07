using System;
using System.Runtime.Serialization;

namespace Microsoft.ConferenceBuddy.Common.Models
{
    [DataContract]
    public class TextAnalyticsResult<T>
    {
        /// <summary>
        /// The results of the Text Analytics result
        /// </summary>
        [DataMember(Name = "documents")]
        public T[] Results { get; set; }
    }

    [DataContract]
    public class TextAnalyticsKeyPhrasesResult
    {
        /// <summary>
        /// The key phrases of the Text Analytics key phrases result
        /// </summary>
        [DataMember(Name = "keyPhrases")]
        public string[] KeyPhrases { get; set; }
    }
}

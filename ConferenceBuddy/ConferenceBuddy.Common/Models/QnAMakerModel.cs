using System;
using System.Runtime.Serialization;

namespace Microsoft.ConferenceBuddy.Common.Models
{
    [DataContract]
    public class QnAMakerSearchRequest
    {
        /// <summary>
        /// The question of the request
        /// </summary>
        [DataMember(Name = "question")]
        public string Question { get; set; }
    }

    [DataContract]
    public class QnAMakerSearchResult
    {
        /// <summary>
        /// The answers of the response
        /// </summary>
        [DataMember(Name = "answers")]
        public QnAMakerResult[] Answers { get; set; }
    }

    [DataContract]
    public class QnAMakerResult
    {
        /// <summary>
        /// The answer of the result
        /// </summary>
        [DataMember(Name = "answer")]
        public string Answer { get; set; }

        /// <summary>
        /// The questions of the result
        /// </summary>
        [DataMember(Name = "questions")]
        public string[] Questions { get; set; }

        /// <summary>
        /// The score of the result
        /// </summary>
        [DataMember(Name = "score")]
        public double Score { get; set; }
    }
}

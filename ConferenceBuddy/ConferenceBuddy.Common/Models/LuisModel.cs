using System;
using System.Runtime.Serialization;

namespace Microsoft.ConferenceBuddy.Common.Models
{
    [DataContract]
    public class LUISResult
    {
        /// <summary>
        /// The query of the result
        /// </summary>
        [DataMember(Name = "query")]
        public string Query { get; set; }

        /// <summary>
        /// The top scoring intent of the result
        /// </summary>
        [DataMember(Name = "topScoringIntent")]
        public LUISIntent TopScoringIntent { get; set; }

        /// <summary>
        /// The intents of the result
        /// </summary>
        [DataMember(Name = "intents")]
        public LUISIntent [] Intents { get; set; }

        /// <summary>
        /// The entities of the result
        /// </summary>
        [DataMember(Name = "entities")]
        public LUISEntity [] Entities { get; set; }
    }

    [DataContract]
    public class LUISEntity
    {
        /// <summary>
        /// The entity
        /// </summary>
        [DataMember(Name = "entity")]
        public string entity { get; set; }

        /// <summary>
        /// The type of the entity
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// The start index of the entity
        /// </summary>
        [DataMember(Name = "startIndex")]
        public int StartIndex { get; set; }

        /// <summary>
        /// The end index of the entity
        /// </summary>
        [DataMember(Name = "endIndex")]
        public int EndIndex { get; set; }

        /// <summary>
        /// The score of the entity
        /// </summary>
        [DataMember(Name = "score")]
        public double Score { get; set; }
    }

    [DataContract]
    public class LUISIntent
    {
        /// <summary>
        /// The intent
        /// </summary>
        [DataMember(Name = "intent")]
        public string Intent { get; set; }

        /// <summary>
        /// The score of the intent
        /// </summary>
        [DataMember(Name = "score")]
        public double Score { get; set; }
    }
}

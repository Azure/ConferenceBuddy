using System;
using System.Runtime.Serialization;

namespace Microsoft.ConferenceBuddy.Common.Models
{
    /// <summary>
    /// Data contract between Bot and Bot Brain for the brain to return a response
    /// </summary>
    [DataContract]
    public class AskQuestionResponse
    {
        /// <summary>
        /// The unique id of the response
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// The results of the response
        /// </summary>
        [DataMember(Name = "results")]
        public AskQuestionResult[] Results { get; set; }
    }
}

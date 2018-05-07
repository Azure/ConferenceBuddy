using System;
using System.Runtime.Serialization;

namespace Microsoft.ConferenceBuddy.Common.Models
{
    /// <summary>
    /// Data contract between Bot and Bot Brain to ask a question
    /// </summary>
    [DataContract]
    public class AskQuestionRequest
    {
        /// <summary>
        /// The session identifier
        /// </summary>
        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }

        /// <summary>
        /// The conversation identifier
        /// </summary>
        [DataMember(Name = "conversationId")]
        public string ConversationId { get; set; }

        /// <summary>
        /// The user identifier
        /// </summary>
        [DataMember(Name = "userId")]
        public string UserId { get; set; }

        /// <summary>
        /// The text of the question
        /// </summary>
        [DataMember(Name = "question")]
        public string Question { get; set; }

        /// <summary>
        /// The topics of the question
        /// </summary>
        [DataMember(Name = "topics")]
        public string[] Topics { get; set; }
    }
}

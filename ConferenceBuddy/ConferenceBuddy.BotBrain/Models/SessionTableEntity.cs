using System;
using System.Linq;

using Microsoft.ConferenceBuddy.Common.Models;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Microsoft.ConferenceBuddy.BotBrain.Models
{
    /// <summary>
    /// Session Telemetry
    /// </summary>
    public class SessionTableEntity : TableEntity
    {
        public SessionTableEntity(string id, DateTime timestampUtc, string questionType, AskQuestionRequest request, AskQuestionResponse response)
        {
            this.PartitionKey = request.SessionId;
            this.RowKey = id;
            this.ConversationId = request.ConversationId;
            this.UserId = request.UserId;
            this.Question = request.Question;
            this.QuestionType = questionType;
            this.Topics = request.Topics?.Count() > 0 ? string.Join(";", request.Topics) : string.Empty;
            this.Timestamp = timestampUtc;
            this.ResponseJson = JsonConvert.SerializeObject(response);
        }

        /// <summary>
        /// Conversation id of the session
        /// </summary>
        public string ConversationId { get; set; }

        /// <summary>
        /// Question by the user
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// The type of question by the user
        /// </summary>
        public string QuestionType { get; set; }

        /// <summary>
        /// Response in json format of to the question
        /// </summary>
        public string ResponseJson { get; set; }

        /// <summary>
        /// Extracted topics of the question
        /// </summary>
        public string Topics { get; set; }

        /// <summary>
        /// A unique identifier of the user that asked the question
        /// </summary>
        public string UserId { get; set; }
    }
}

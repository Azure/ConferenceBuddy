using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.ConferenceBuddy.Common.Models;

namespace Microsoft.ConferenceBuddy.BotBrain.Models
{
    [SerializePropertyNamesAsCamelCase]
    public abstract class SearchDocument
    {
        public SearchDocument(string id)
        {
            this.Id = id;
        }

        [Key]
        [IsRetrievable(true)]
        public string Id { get; set; }
    }

    /// <summary>
    /// Session document for Azure search
    /// </summary>
    [SerializePropertyNamesAsCamelCase]
    public class SessionSearchDocument : SearchDocument
    {
        public SessionSearchDocument(string id, DateTime timestampUtc, string task,
            AskQuestionRequest request, AskQuestionResponse response)
            : base(id)
        {
            this.SessionId = request.SessionId;
            this.ConversationId = request.ConversationId;
            this.UserId = request.UserId;
            this.Question = request.Question;
            this.Task = task;
            this.Topics = request.Topics;
            this.TimestampUtc = timestampUtc;

            if (response.Results?.Count() > 0)
            {
                this.AnswerTitle = response.Results[0].Title;
                this.AnswerContent = response.Results[0].Answer;
                this.AnswerImageUrl = response.Results[0].ImageUrl;
                this.AnswerSource = response.Results[0].Source;
                this.AnswerUrl = response.Results[0].Url;
                this.AnswerUrlDisplayName = response.Results[0].UrlDisplayName;
            }

            this.IsAnswered = false;
        }

        public SessionSearchDocument(string id)
            : base(id)
        {

        }

        /// <summary>
        /// Identifier of the session
        /// </summary>
        [IsRetrievable(true), IsFilterable, IsFacetable]
        public string SessionId { get; set; }

        /// <summary>
        /// Identifier of the conversation
        /// </summary>
        [IsRetrievable(true), IsFilterable, IsFacetable]
        public string ConversationId { get; set; }

        /// <summary>
        /// Identifier of the user
        /// </summary>
        [IsRetrievable(true), IsFilterable, IsFacetable]
        public string UserId { get; set; }

        /// <summary>
        /// Question that the user have asked
        /// </summary>
        [IsRetrievable(true), IsSearchable]
        public string Question { get; set; }

        /// <summary>
        /// The Bot task used for this session
        /// </summary>
        [IsRetrievable(true), IsFilterable, IsFacetable]
        public string Task { get; set; }

        /// <summary>
        /// The topics extracted from the question
        /// </summary>
        [IsRetrievable(true), IsFilterable, IsFacetable]
        public string[] Topics { get; set; }

        /// <summary>
        /// The UTC timestamp of the question
        /// </summary>
        [IsRetrievable(true), IsFilterable, IsSortable, IsFacetable]
        public DateTimeOffset? TimestampUtc { get; set; }

        /// <summary>
        /// Title of the answer to the question
        /// </summary>
        [IsRetrievable(true), IsSearchable]
        public string AnswerTitle { get; set; }

        /// <summary>
        /// Content of the answer to the question
        /// </summary>
        [IsRetrievable(true), IsSearchable]
        public string AnswerContent { get; set; }

        /// <summary>
        /// Image of the answer to the question
        /// </summary>
        [IsRetrievable(true)]
        public string AnswerImageUrl { get; set; }
        
        /// <summary>
        /// Source of the answer to the question
        /// </summary>
        [IsRetrievable(true), IsFilterable, IsFacetable]
        public string AnswerSource { get; set; }

        /// <summary>
        /// Url of the answer to the question
        /// </summary>
        [IsRetrievable(true)]
        public string AnswerUrl { get; set; }

        /// <summary>
        /// Display name of the answer url
        /// </summary>
        [IsRetrievable(true)]
        public string AnswerUrlDisplayName { get; set; }

        /// <summary>
        /// Mark if a question have been answered by the speaker
        /// </summary>
        [IsRetrievable(true), IsFilterable, IsFacetable]
        public bool IsAnswered { get; set; }
    }
}

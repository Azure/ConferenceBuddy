using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ConferenceBuddy.BotBrain.Models;
using Microsoft.ConferenceBuddy.BotBrain.Utilities;
using Microsoft.ConferenceBuddy.Common.Models;

namespace Microsoft.ConferenceBuddy.BotBrain
{
    public static class AskLearnMoreTask
    {
        /// <summary>
        /// Bot Task: Ask Learn More
        /// Handles request with questions related to a person and respond with result either from QnA or Bing Search
        /// </summary>
        public static class AskWhoTask
        {
            [FunctionName("AskLearnMore")]
            public static async Task<HttpResponseMessage> Run(
                [HttpTrigger(AuthorizationLevel.Function, "post", Route = "AskLearnMore")]HttpRequestMessage request,
                [Table("Session", Connection = "AzureWebJobsStorage")]ICollector<SessionTableEntity> sessionTable,
                TraceWriter log)
            {
                // Get content type
                MediaTypeHeaderValue contentType = request.Content.Headers.ContentType;

                // Check if content type is empty
                if (contentType == null)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Missing content-type from header.");
                }
                else if (contentType.MediaType.Contains("application/json") == false)
                {
                    return request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType,
                        string.Format("Request's content type ({0}) is not supported.", string.Join(", ", contentType.MediaType)));
                }

                // Read content from request
                AskQuestionRequest requestBody = await request.Content.ReadAsAsync<AskQuestionRequest>();

                // Verify content contains valid values
                if (string.IsNullOrEmpty(requestBody.Question) == true)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Question is missing from the request content.");
                }
                else if (string.IsNullOrEmpty(requestBody.SessionId) == true)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Session id is missing from the request content.");
                }

                // Answer the question
                AskQuestionResponse response = await AnswerQuestion(requestBody, sessionTable);

                // Return request response with result and 200 OK
                return request.CreateResponse(HttpStatusCode.OK, response);
            }

            public static async Task<AskQuestionResponse> AnswerQuestion(AskQuestionRequest request, ICollector<SessionTableEntity> sessionTable)
            {
                // Get unique identifier
                string id = Guid.NewGuid().ToString();
                DateTime timestampUtc = DateTime.UtcNow;

                // Get key phrases extraction
                request.Topics = await ServicesUtility.GetTopics(request.Question, request.Topics);

                string queryWithTopics = request.Topics?.Count() > 0 ? string.Join(" ", request.Topics).Trim() : request.Question;

                // Create video indexer task
                VideoIndexerSearchResult videoIndexerResult = await ServicesUtility.VideoIndexer.SearchAsync(query: queryWithTopics);
                

                // Process results
                AskQuestionResponse response = new AskQuestionResponse()
                {
                    Id = id,
                    Results = new AskQuestionResult[0]
                };

                if (videoIndexerResult.Results?.Count() > 0)
                {
                    response.Results = ServicesUtility.GetAskQuestionResults(videoIndexerResult);
                }
                else
                {
                    // Create Bing Search tasks
                    BingWebSearchResult bingWebSearchResult =
                        await ServicesUtility.BingSearch.SearchWebAsync(query: request.Question, count: SettingsUtility.MaxResultsCount);

                    // Process Bing search results
                    if (bingWebSearchResult.WebPagesResult?.Values?.Count() > 0)
                    {
                        response.Results = ServicesUtility.GetAskQuestionResults(bingWebSearchResult);
                    }
                }
                
                // Write to session table
                sessionTable.Add(new SessionTableEntity(id, timestampUtc, "LearnMore", request, response));

                // Return response
                return response;
            }
        }
    }
}

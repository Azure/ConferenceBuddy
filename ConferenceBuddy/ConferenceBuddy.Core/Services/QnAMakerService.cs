using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.ConferenceBuddy.Common.Models;
using Microsoft.ConferenceBuddy.Common.Utilities;

namespace Microsoft.ConferenceBuddy.Core.Services
{
    public class QnAMakerService : ServiceBase
    {
        /// <summary>
        /// The constructor of the QnA Maker service
        /// </summary>
        public QnAMakerService(string serviceUrl, string subscriptionKey)
        {
            if (string.IsNullOrEmpty(serviceUrl) == true)
            {
                throw new ArgumentNullException("Service url is not initialized.");
            }

            if (string.IsNullOrEmpty(subscriptionKey) == true)
            {
                throw new ArgumentNullException("Subscription key is not initialized.");
            }

            this.BaseServiceUrl = serviceUrl;
            this.RequestHeaders = new Dictionary<string, string>()
            {
                {  this.HEADER_SUB_KEY, subscriptionKey }
            };
        }

        /// <summary>
        /// Generate answer from QnA Maker service
        /// </summary>
        public async Task<QnAMakerSearchResult> GenerateAnswerAsync(string query, string knowledgebaseId)
        {
            if (string.IsNullOrEmpty(query) == true)
            {
                return default(QnAMakerSearchResult);
            }

            // Get request uri
            string requestUrl = Path.Combine(this.BaseServiceUrl, "knowledgebases", knowledgebaseId, "generateAnswer");
            Uri requestUri = new Uri(requestUrl);

            // Create content
            QnAMakerSearchRequest content = new QnAMakerSearchRequest()
            {
                Question = query
            };

            // Post request
            QnAMakerSearchResult result = await HttpClientUtility.PostAsJsonAsync<QnAMakerSearchResult>(requestUri, this.RequestHeaders, content);
            return result;
        }

    }
}

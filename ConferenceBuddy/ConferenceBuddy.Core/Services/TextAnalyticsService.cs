using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.ConferenceBuddy.Common.Models;
using Microsoft.ConferenceBuddy.Common.Utilities;

namespace Microsoft.ConferenceBuddy.Core.Services
{
    public class TextAnalyticsService : ServiceBase
    {
        /// <summary>
        /// The constructor of the Text Analytics Service
        /// </summary>
        public TextAnalyticsService(string serviceUrl, string subscriptionKey)
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
        /// Analyze key phrases with text
        /// </summary>
        public async Task<TextAnalyticsResult<TextAnalyticsKeyPhrasesResult>> AnalyzeKeyPhrasesAsync(string fullText)
        {
            if (string.IsNullOrEmpty(fullText) == true)
            {
                return default(TextAnalyticsResult<TextAnalyticsKeyPhrasesResult>);
            }

            // Get request uri
            Uri requestUri = new Uri(this.BaseServiceUrl + "keyPhrases");

            var document = new
            {
                id = Guid.NewGuid().ToString(),
                text = fullText
            };

            // Create content of the request
            var content = new
            {
                documents = new object[] { document }
            };

            // Get response
            return await HttpClientUtility.PostAsJsonAsync<TextAnalyticsResult<TextAnalyticsKeyPhrasesResult>>(requestUri, this.RequestHeaders, content);
        }
    }
}

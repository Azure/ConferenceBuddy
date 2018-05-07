using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Microsoft.ConferenceBuddy.Common.Models;
using Microsoft.ConferenceBuddy.Common.Utilities;

namespace Microsoft.ConferenceBuddy.Core.Services
{
    public class LUISService : ServiceBase
    {
        /// <summary>
        /// The constructor of the LUIS Service
        /// </summary>
        public LUISService(string serviceUrl, string appId, string subscriptionKey, string spellcheckSubscriptionKey)
        {
            if (string.IsNullOrEmpty(serviceUrl) == true)
            {
                throw new ArgumentNullException("Service url is not initialized.");
            }

            if (string.IsNullOrEmpty(appId) == true)
            {
                throw new ArgumentNullException("App id is not initialized.");
            }

            if (string.IsNullOrEmpty(subscriptionKey) == true)
            {
                throw new ArgumentNullException("Subscription key is not initialized.");
            }

            if (string.IsNullOrEmpty(spellcheckSubscriptionKey) == true)
            {
                throw new ArgumentNullException("Spellcheck Subscription key is not initialized.");
            }

            this.BaseServiceUrl = string.Format(serviceUrl + "{0}?subscription-key={1}&spellCheck=true&bing-spell-check-subscription-key={2}", appId, subscriptionKey, spellcheckSubscriptionKey);
        }

        /// <summary>
        /// Query LUIS service to obtain results
        /// </summary>
        public async Task<LUISResult> QueryAsync(string query)
        {
            if (string.IsNullOrEmpty(query) == true)
            {
                return default(LUISResult);
            }

            query = WebUtility.UrlEncode(query);

            // Get request uri
            string requestUrl = this.BaseServiceUrl + "&verbose=true&timezoneOffset=0&q=" + query;
            Uri requestUri = new Uri(requestUrl);

            LUISResult result = await HttpClientUtility.GetAsync<LUISResult>(requestUri, this.RequestHeaders);
            return result;
        }
    }
}
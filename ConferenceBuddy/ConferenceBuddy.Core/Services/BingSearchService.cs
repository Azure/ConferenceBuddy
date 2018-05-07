using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Microsoft.ConferenceBuddy.Common.Models;
using Microsoft.ConferenceBuddy.Common.Utilities;

namespace Microsoft.ConferenceBuddy.Core.Services
{
    public class BingSearchService : ServiceBase
    {
        public enum SafeSearch
        {
            Off, Moderate, Strict
        }

        /// <summary>
        /// The constructor of the Bing Search Service
        /// </summary>
        public BingSearchService(string serviceUrl, string subscriptionKey)
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
        /// Search web pages with Bing service
        /// </summary>
        public async Task<BingWebSearchResult> SearchWebAsync(string query, int count = 10, int offset = 0, string market = "en-us", SafeSearch safeSearch = SafeSearch.Strict)
        {
            if (string.IsNullOrEmpty(query) == true)
            {
                return default(BingWebSearchResult);
            }
            else if (query.Length > MAX_QUERY_LENGTH)
            {
                query = query.Substring(0, MAX_QUERY_LENGTH);
            }

            query = WebUtility.UrlEncode(query);

            // Get request uri
            string searchUrl = string.Format(this.BaseServiceUrl + "search?q={0}&count={1}&offset={2}&mkt={3}&safesearch={4}", query, count, offset, market, safeSearch);
            Uri requestUri = new Uri(searchUrl);

            // Get response
            BingWebSearchResult result = await HttpClientUtility.GetAsync<BingWebSearchResult>(requestUri, this.RequestHeaders);
            return result;
        }

    }
}

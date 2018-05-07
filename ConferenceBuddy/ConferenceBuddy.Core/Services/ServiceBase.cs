using System;
using System.Collections.Generic;

namespace Microsoft.ConferenceBuddy.Core.Services
{
    public abstract class ServiceBase
    {
        /// <summary>
        /// Common Http request header for Cognitive Services
        /// </summary>
        public readonly string HEADER_SUB_KEY = "Ocp-Apim-Subscription-Key";

        /// <summary>
        /// Define the max length of the query
        /// </summary>
        public readonly int MAX_QUERY_LENGTH = 1000;

        /// <summary>
        /// Define the base dictionary that contains the request headers
        /// </summary>
        public IDictionary<string, string> RequestHeaders { get; protected set; }

        /// <summary>
        /// Define the service url
        /// </summary>
        public string BaseServiceUrl { get; protected set; }
    }
}

using System;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace PresentationToolkit.Core.HttpRequests
{
    /// <summary>
    /// Uri http Get/Post related extension methods.
    /// </summary>
    public static class UriRequestExtensions
    {
        /*
        private static bool ShouldIncludeProperty(string name)
        {
            return name != "IsMasked" 
                && name != "IsManual" 
                && name != "IsDeleted" 
                && name != "IsClipped"
                && name != "HasAudit";
        }

        private static Type GetPropertyType(string name)
        {
            if (name == "LastModified"
                || name == "CreatedDateTime"
                || name == "ConfirmedDateTime"
                || name == "EndDateTime")
            {
                return typeof(DateTime);
            }
            if (name == "IsVirtual"
                || name == "IsSplit")
            {
                return typeof (bool);
            }

            return typeof (string);
        }
        public static string SplitCamelCase(string stringValue)
        {
            return Regex.Replace(Regex.Replace(stringValue, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }
        
        */
        
        /// <summary>
        /// Requests the Http Get for the uri and deserialized the contents by JSON.
        /// </summary>
        /// <param name="uri">The service uri.</param>
        /// <returns>The deserialized object by JSON from response contents.</returns>
        public static async Task<T> GetAsync<T>(this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri", nameof(uri));
            }

            string content  = await HttpClientProxy.GetAsync(uri);
            T result = await Task.Run<T>(() => JsonConvert.DeserializeObject<T>(content));

            return result;
        }

        /// <summary>
        /// Requests the Http Get for the uri and reads the contents.
        /// </summary>
        /// <param name="uri">The service uri.</param>
        /// <returns>The response contents.</returns>
        public static Task<string> GetAsync(this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri", nameof(uri));
            }

            return HttpClientProxy.GetAsync(uri);
        }

        /// <summary>
        /// Requests the Http Post for the uri and deserialized the contents by JSON.
        /// </summary>
        /// <param name="uri">The service uri.</param>
        /// <param name="postContent">The <see cref="HttpContent"/> to post.</param>
        /// <returns>The deserialized object by JSON from response contents.</returns>
        public static async Task<T> PostAsync<T>(this Uri uri, HttpContent postContent = null)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri", nameof(uri));
            }

            string content = await HttpClientProxy.PostAsync(uri, postContent);
            T result = await Task.Run<T>(() => JsonConvert.DeserializeObject<T>(content));

            return result;
        }

        /// <summary>
        /// Requests the Http Get for the uri and reads the contents.
        /// </summary>
        /// <param name="uri">The service uri.</param>
        /// <param name="postContent">The <see cref="HttpContent"/> to post.</param>
        /// <returns>The response contents.</returns>
        public static Task<string> PostAsync(this Uri uri, HttpContent postContent = null)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri", nameof(uri));
            }

            return HttpClientProxy.PostAsync(uri, postContent);
        }
    }
}

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PresentationToolkit.Core.NetPoxy
{
    /// <summary>
    /// Provides client http requests and responses.
    /// </summary>
    public sealed class HttpClientProxy : IDisposable
    {
        private readonly Uri uri;
        private readonly HttpClient client;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpClientProxy"/>.
        /// </summary>
        /// <param name="uri">The service uri.</param>
        internal HttpClientProxy(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri", nameof(uri));
            }

            this.uri = uri;

            client = new HttpClient();
            isDisposed = false;
        }

        /// <summary>
        /// Requests the Http Get for the uri and reads the contents.
        /// </summary>
        /// <param name="uri">The service uri.</param>
        /// <returns>The response contentss.</returns>
        public static async Task<string> GetAsync(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri", nameof(uri));
            }

            string content = null;
            using (HttpClientProxy proxy = new HttpClientProxy(uri))
            {
                content = await proxy.GetAsync();
            }

            return content;
        }

        /// <summary>
        /// Requests the Http Post for the uri and reads the contents.
        /// </summary>
        /// <param name="uri">The service uri.</param>
        /// <param name="postContent">The <see cref="HttpContent"/> to post.</param>
        /// <returns>The response contentss.</returns>
        public static async Task<string> PostAsync(Uri uri, HttpContent postContent = null)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri", nameof(uri));
            }

            string content = null;
            using (HttpClientProxy proxy = new HttpClientProxy(uri))
            {
                content = await proxy.PostAsync(postContent);
            }

            return content;
        }

        /// <summary>
        /// Requests the Http Get for the uri and reads the contents.
        /// </summary>
        /// <returns>The response contentss.</returns>
        public async Task<string> GetAsync()
        {
            HttpResponseMessage response = null;
            try
            {
                response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                return content;
            }
            finally
            {
                if (response != null)
                {
                    response.Dispose();
                }
            }
        }

        /// <summary>
        /// Requests the Http Post for the uri and reads the contents.
        /// </summary>
        /// <param name="postContent">The <see cref="HttpContent"/> to post.</param>
        /// <returns>The response contentss.</returns>
        public async Task<string> PostAsync(HttpContent postContent = null)
        {
            HttpResponseMessage response = null;
            try
            {
                response = await client.PostAsync(uri, postContent);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                return content;
            }
            finally
            {
                if (response != null)
                {
                    response.Dispose();
                }
            }
        }

        /// <summary>
        /// Disposes the current instance.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            client.Dispose();
        }
    }
}

using System;
using System.Json;
using System.Net;
using System.Threading.Tasks;
using MvvmCross.Platform;
using Translate.Core.Contracts;

namespace Translate.Services
{
    public class WebRequestService : IWebRequestService
    {
        public async Task<string> GetResponseAsync(string url)
        {
            // Create an HTTP web request using the URL:
            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (var response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (var stream = response.GetResponseStream())
                {
                    // Use this stream to build a JSON document object:
                    var jsonDoc = await Task.Run(() => JsonValue.Load(stream));
                    Mvx.Trace("Response: {0}", jsonDoc);

                    // Return the JSON document:
                    return jsonDoc.ToString();
                }
            }
        }
    }
}

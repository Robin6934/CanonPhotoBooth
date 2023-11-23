using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PhotoBooth
{
    internal class RestApi
    {

        /// <summary>
        /// Make a GET request
        /// </summary>
        /// <param name="Url">The URL for the Request</param>
        /// <returns></returns>
        public static async Task RestApiGet(string Url)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = Url; // Replace with your API URL

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    ReportError("Error: " + response.StatusCode);
                }
            }
        }

        /// <summary>
        /// Make a GET request and return the response
        /// </summary>
        /// <param name="Url">The URL for the Request</param>
        /// <returns>Returns the response as HttpResponseMessage</returns>
        public static async Task<HttpResponseMessage> RestApiGetReturn(string Url)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = Url; // Replace with your API URL

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                return response;
            }
        }

        /// <summary>
        /// Post a new picture to the Spring Boot API
        /// </summary>
        /// <param name="Url">The Url of the Endpoint for the Init</param>
        /// <param name="photoBoothInit">The DTO with the data to send to the Spring application</param>
        /// <returns></returns>
        public static async Task RestApiPost(string Url, PhotoBoothInit photoBoothInit)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = Url; // Replace with your API URL

                // Create a JSON payload (replace with your data)
                string jsonPayload = JsonSerializer.Serialize(photoBoothInit);

                // Set the content type to JSON
                StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    ReportError("Error: " + response.StatusCode);
                }
            }
        }

        public static void ReportError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// The DTO for the init
    /// </summary>
    public record PhotoBoothInit
    {
        public string? picturePath { get; set; }
    }


    /// <summary>
    /// The DTO for the poling
    /// </summary>
    public record PolingDTO
    {
        public bool triggerPicture { get; set; }
        public string? printPictureName { get; set; }
    }
}

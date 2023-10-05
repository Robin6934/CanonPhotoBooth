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

        public static async Task<HttpResponseMessage> RestApiGetReturn(string Url)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = Url; // Replace with your API URL

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                return response;
            }
        }

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

    public record PhotoBoothInit
    {
        public string picturePath { get; set; }
    }
}

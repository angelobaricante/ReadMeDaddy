using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReadMeDaddy
{
    public class ApiHandler
    {
        private HttpClient client = new HttpClient();

        public ApiHandler(string apiKey)
        {
            InitializeHttpClient(apiKey);
        }

        private void InitializeHttpClient(string apiKey)
        {
            client.BaseAddress = new Uri("https://api.openai.com/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<string> SendRequestToOpenAI(string text, string prompt)
        {
            var requestData = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                new { role = "system", content = prompt },
                new { role = "user", content = text }
            }
            };

            string json = JsonConvert.SerializeObject(requestData);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("v1/chat/completions", content);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
                return responseObject.choices[0].message.content.ToString();
            }
            else
            {
                return $"Error: {response.StatusCode} - {responseContent}";
            }
        }
    }
}
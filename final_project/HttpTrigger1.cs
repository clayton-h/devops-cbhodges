using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoMesa.TestFunction1
{
    public static class HttpTrigger1
    {
        private static readonly HttpClient httpClient = new HttpClient();

        // Retrieve the OpenAI API key from the environment variables.
        private static readonly string openaiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.Process);
        private static readonly string openaiApiUrl = "https://api.openai.com/v1/engines/davinci/completions";

        [FunctionName("HttpTrigger1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string body = data?.body;

            if (string.IsNullOrWhiteSpace(body))
            {
                return new BadRequestObjectResult("Please pass the email content in the request body.");
            }

            log.LogInformation("Email content sent to ChatGPT.");

            var sentimentResult = await GetSentiment(body, log);

            log.LogInformation("Sentiment returned from ChatGPT.");

            return new OkObjectResult(sentimentResult);
        }
        private static async Task<string> GetSentiment(string emailContent, ILogger log)
        {
            var sentimentRequestBody = new
            {
                prompt = $"Provide a single word to describe the sentiment of this email: \"{emailContent}\"",
                max_tokens = 5
            };

            string sentimentRequestBodyJson = JsonConvert.SerializeObject(sentimentRequestBody);
            var requestContent = new StringContent(sentimentRequestBodyJson, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", openaiApiKey);

            var response = await httpClient.PostAsync(openaiApiUrl, requestContent);
            if (!response.IsSuccessStatusCode)
            {
                log.LogError($"OpenAI API call failed: {response.StatusCode}");
                return "Error in analyzing sentiment";
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseJson = JsonConvert.DeserializeObject(responseContent);

            string sentiment = responseJson.choices[0].text;

            return sentiment.Trim();
        }
    }
}

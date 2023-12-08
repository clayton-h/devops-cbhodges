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

        private static readonly string openaiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.Process);
        private static readonly string openaiApiUrl = "https://api.openai.com/v1/engines/text-davinci-003/completions";

        [FunctionName("HttpTrigger1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string body = data?.body;

            if (string.IsNullOrWhiteSpace(body))
            {
                return new BadRequestObjectResult("Please pass the text content in the request body.");
            }

            log.LogInformation("Text content sent to OpenAI for analysis.");

            var analysisResult = await GetSummary(body, log);

            log.LogInformation("Summary returned from OpenAI.");

            return new OkObjectResult(analysisResult);
        }

        private static async Task<string> GetSummary(string textContent, ILogger log)
        {
            var analysisRequestBody = new
            {
                prompt = $"Summarize the following text and provide a JSON response with two fields (one with sentiment of 'positive', 'negative', or 'other' and one with an analysis): \"{textContent}\"",
                max_tokens = 100
            };

            string analysisRequestBodyJson = JsonConvert.SerializeObject(analysisRequestBody);
            var requestContent = new StringContent(analysisRequestBodyJson, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", openaiApiKey);

            var response = await httpClient.PostAsync(openaiApiUrl, requestContent);
            if (!response.IsSuccessStatusCode)
            {
                log.LogError($"OpenAI API call failed: {response.StatusCode}");
                return "Error in summarizing text";
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseJson = JsonConvert.DeserializeObject(responseContent);

            string summary = responseJson.choices[0].text;

            return summary.Trim();
        }
    }
}

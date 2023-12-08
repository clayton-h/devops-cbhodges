# Integrating OpenAI with Azure Functions

This guide outlines the steps to create an Azure Function App for an HTTP trigger that integrates with OpenAI.

## 1. Creating an Azure Function App for HTTP Trigger via Azure Portal

- **Step 1**: Log in to the Azure Portal.
- **Step 2**: Navigate to "Function Apps" and click on "Create".
- **Step 3**: Fill in the necessary details like subscription, resource group, and instance details.
- **Step 4**: Choose the runtime stack (e.g., .NET) and the region.
- **Step 5**: Review and create the function app.

## 2. Setting up a Local Visual Studio Code Project with Azure Functions Extension

- **Step 1**: Install Visual Studio Code and the Azure Functions extension.
- **Step 2**: Open VS Code and create a new project.
- **Step 3**: Select the language (e.g., C#) and template (HTTP trigger).
- **Step 4**: Configure the local settings, such as the connection string.

## 3. Modifying the Generated Basic Function Code for OpenAI Integration

- **Step 1**: Open the generated function code in VS Code.
- **Step 2**: Include necessary namespaces for HTTP requests and JSON handling.
- **Step 3**: Define variables for OpenAI API key and endpoint.
- **Step 4**: Create an HTTP client for sending requests to OpenAI.
- **Step 5**: Add a method to construct the OpenAI request with the prompt and token limit.
- **Step 6**: Handle the response and extract the desired information.

## 4. Choosing the Correct AI Model

- Determine the appropriate model based on the task (e.g., text-davinci-003 for complex tasks).

## 5. Specifying the AI Prompt and Its Length

- Set the prompt based on the input requirement.
- Limit the response length using `max_tokens`.

## 6. Adding OpenAI API Key and Logging Content

- Add your OpenAI API key to the Azure Function's configuration.
- Implement logging to track requests and responses.

## Code Example

```csharp
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

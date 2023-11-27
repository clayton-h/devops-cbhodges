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

public static class EmailSentimentChecker
{
    private static readonly HttpClient httpClient = new HttpClient();
    
    // Retrieve the OpenAI API key from the environment variables.
    private static readonly string openaiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.Process);
    private static readonly string openaiApiUrl = "https://api.openai.com/v1/engines/davinci/completions";

    [FunctionName("EmailSentimentChecker")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        string emailContent = data?.emailContent;

        if (string.IsNullOrWhiteSpace(emailContent))
        {
            return new BadRequestObjectResult("Please pass the email content in the request body");
        }

        var sentimentResult = await GetSentiment(emailContent);
        return new OkObjectResult(sentimentResult);
    }

    private static async Task<string> GetSentiment(string text)
    {
        var requestData = new
        {
            model = "text-davinci-003",
            prompt = $"Analyze the sentiment of this email: \"{text}\"",
            temperature = 0,
            max_tokens = 60
        };

        var jsonContent = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        content.Headers.Add("Authorization", $"Bearer {openaiApiKey}");

        var response = await httpClient.PostAsync(openaiApiUrl, content);
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }
}

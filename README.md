# Integrating OpenAI with Azure Functions

This repository serves as a personal project to integrate OpenAI with Azure Functions using an HTTP trigger. It outlines the steps for setting up an Azure Function App and sending requests to OpenAI’s API for tasks such as text summarization and sentiment analysis.

## Purpose

This project is primarily for personal experimentation and learning, focusing on:
- Creating and deploying an Azure Function with an HTTP trigger.
- Interacting with OpenAI's API to process requests and handle responses.
- Exploring the use of different AI models for various tasks.

## Project Breakdown

1. **Creating an Azure Function App for HTTP Trigger via Azure Portal**
   - Step-by-step instructions for setting up the function app in the Azure Portal.

2. **Setting up a Local Visual Studio Code Project with Azure Functions Extension**
   - Configuring your local development environment for Azure Functions using VS Code.

3. **Modifying the Basic Function for OpenAI Integration**
   - Code for integrating with OpenAI's API, handling requests and responses.

4. **Selecting the Right AI Model and Prompt**
   - Guidelines for choosing the appropriate model and formatting prompts.

5. **Adding OpenAI API Key and Logging**
   - How to configure your OpenAI API key and implement logging to track requests.
  
## ChatGPT Sentiment Analysis V1.0

Released on **Dec 11, 2023**, this project integrates with ChatGPT to perform sentiment analysis on input text. The Azure Function sends text to OpenAI's ChatGPT model and retrieves sentiment analysis results. The function is tested in the Azure portal environment but has not yet been integrated into a Logic App.

### Example:
**Input:**
```json
{
  "body": "The very best."
}
```
**Output:**
```
{
  "sentiment": "Positive",
  "analysis": "The speaker is expressing absolute approval."
}
```

## Detailed Instructions

All detailed instructions and code examples for this project are located in the `final_project` directory. If you're looking for specifics on how the project is set up, please visit the guide there:

👉 **[Go to the Final Project Guide](./final_project/readme.md)**

## Project Structure

```
/
├── (other coursework)
├── azure_sentiment/
│   ├── function.json         # Function configuration for Azure
│   ├── function.proj         # Project file for Azure Function
│   ├── project.assets.json   # Additional project assets
│   └── run.csx               # Core Azure Function script for sentiment analysis using OpenAI
├── final_project/
│   ├── (various configuration files)  # Configuration files
│   └── readme.md                      # Detailed guide for setting up the project
└── README.md                 # Overview of the repository
```

using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using NativeFunctionDemo.Plugins;

IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

AzureKeyCredential azureKeyCredential = new(config["AzureOpenAI:AzureKeyCredential"]!);
string deploymentName = config["AzureOpenAI:DeploymentName"]!;
Uri endpoint = new Uri(config["AzureOpenAI:Endpoint"]!);
OpenAIClient openAIClient = new(endpoint, azureKeyCredential);

IKernelBuilder builder = Kernel.CreateBuilder();

//builder.Services.AddLogging((options) =>
//{
//    options.SetMinimumLevel(LogLevel.Trace);
//    options.AddConsole();
//});

builder.Services.AddAzureOpenAIChatCompletion(deploymentName, openAIClient);

var kernel = builder.Build();

// Registering plugins to the kernel
kernel.Plugins.AddFromType<MathPlugin>();
kernel.Plugins.AddFromType<TrainPlugin>();

// Create chat history
ChatHistory history = [];

// Retrieve the chat completion service from the kernel
IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Enable auto function calling
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

//// Start the conversation
while (true)
{
  // Get user input
  Console.ForegroundColor = ConsoleColor.Green;
  Console.Write("User > ");
  history.AddUserMessage(Console.ReadLine()!);

  // Get the response from the AI
  IAsyncEnumerable<StreamingChatMessageContent> result = chatCompletionService.GetStreamingChatMessageContentsAsync(history, kernel: kernel, executionSettings: openAIPromptExecutionSettings);

  // Stream the results
  string fullMessage = "";
  Console.ForegroundColor = ConsoleColor.Cyan;
  Console.Write("Assistant > ");
  await foreach (var content in result)
  {
    Console.Write(content.Content);
    fullMessage += content.Content;
  }
  Console.WriteLine();

  // Add the message from the agent to the chat history
  history.AddAssistantMessage(fullMessage);
}
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

// Loading configuration
IConfigurationRoot config = new ConfigurationBuilder()
                                .AddUserSecrets<Program>()
                                .Build();

AzureKeyCredential azureKeyCredential = new(config["AzureOpenAI:AzureKeyCredential"]!);
string deploymentName = config["AzureOpenAI:DeploymentName"]!;
Uri endpoint = new Uri(config["AzureOpenAI:Endpoint"]!);

OpenAIClient openAIClient = new(endpoint, azureKeyCredential);

IKernelBuilder builder = Kernel.CreateBuilder();

builder.Services.AddAzureOpenAIChatCompletion(deploymentName, openAIClient);

Kernel kernel = builder.Build();

// Create chat history
ChatHistory history = [];
history.AddSystemMessage("You should respond in a shakespearean way to all further requests");

// Retrieve the chat completion service from the kernel
IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

//// Start the conversation
while (true)
{
  // Get user input
  Console.ForegroundColor = ConsoleColor.Green;
  Console.Write("User > ");
  history.AddUserMessage(Console.ReadLine()!);

  // Get the response from the AI
  IAsyncEnumerable<StreamingChatMessageContent> result = chatCompletionService.GetStreamingChatMessageContentsAsync(history, kernel: kernel);

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
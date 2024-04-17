using Azure;
using Azure.AI.OpenAI;
using Azure.Core;
using Client.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using System.Reflection;


(AzureKeyCredential? azureKeyCredential, string? deploymentName, Uri? endpoint) = LoadConfiguration();
OpenAIClient openAIClient = new(endpoint, azureKeyCredential);

IKernelBuilder builder = Kernel.CreateBuilder();
builder.Services.AddAzureOpenAIChatCompletion(deploymentName, openAIClient);

builder.Plugins.AddFromType<CoursePlugin>();

Kernel kernel = builder.Build();

// Load prompt from YAML
var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Client.Prompts.courseSelector.prompt.yaml")!;
using StreamReader reader = new(stream);
KernelFunction courseSelector = kernel.CreateFunctionFromPromptYaml(
    reader.ReadToEnd(),
    promptTemplateFactory: new HandlebarsPromptTemplateFactory()
);

kernel.Plugins.AddFromFunctions("CourseSelector", [courseSelector]);

// Create the chat history
ChatHistory chatMessages = new ChatHistory();

chatMessages.AddSystemMessage("""
You are a friendly assistant who likes to follow the rules. You will complete required steps
and request approval before taking any consequential actions. If the user doesn't provide
enough information for you to complete a task, you will keep asking questions until you have
enough information to complete the task. You are not allowed to answer any questions that are unrelated
to U2U courses!
""");

// Retrieve the chat completion service from the kernel
IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Make sure the LLM can automatically invoke our plugin functions
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
};

// Start the conversation
while (true)
{
    // Get user input
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("User > ");
    chatMessages.AddUserMessage(Console.ReadLine()!);

    // Get the chat completions
    
    IAsyncEnumerable<StreamingChatMessageContent> result = chatCompletionService.GetStreamingChatMessageContentsAsync(
        chatMessages,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    // Stream the results
    string fullMessage = "";
    await foreach (StreamingChatMessageContent content in result)
    {
        if (content.Role.HasValue)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Assistant > ");
        }
        Console.Write(content.Content);
        fullMessage += content.Content;
    }
    Console.WriteLine();

    // Add the message from the agent to the chat history
    chatMessages.AddAssistantMessage(fullMessage);
}

(AzureKeyCredential? azureKeyCredential, string? deploymentName, Uri? endpoint) LoadConfiguration()
{
    IConfigurationRoot config = new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .Build();

    AzureKeyCredential? azureKeyCredential = new(config["AzureOpenAI:AzureKeyCredential"]);
    string? deploymentName = config["AzureOpenAI:DeploymentName"];
    Uri? endpoint = new Uri(config["AzureOpenAI:Endpoint"]);

    return (azureKeyCredential,  deploymentName, endpoint);
}
using Azure;
using Azure.AI.OpenAI;
using Azure.Core;
using ConsoleTables;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using NL2SQL.Terminal.Configuration;
using NL2SQL.Terminal.Infra;
using System.Data;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Xml;
using NL2SQL.Infra;
using NL2SQL.Terminal.NativeFunctions;

(IConfiguration config, AzureKeyCredential? azureKeyCredential, string? deploymentName, Uri? endpoint) = SetUp.LoadConfiguration();
OpenAIClient openAIClient = new(endpoint, azureKeyCredential);

IKernelBuilder builder = Kernel.CreateBuilder();

builder.Services.AddAzureOpenAIChatCompletion(deploymentName, openAIClient);
builder.Services.AddSingleton<DapperConnectionProvider>();
builder.Services.AddSingleton<QueryExecutor>();
builder.Services.AddSingleton<IConfiguration>(config);

Kernel kernel = builder.Build();

// Load prompt from YAML
var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NL2SQL.Terminal.Prompts.nl2tsqlTranslator.prompt.yaml")!;
using StreamReader reader = new(stream);

KernelFunction nl2TsqlTranslator = kernel.CreateFunctionFromPromptYaml(
    reader.ReadToEnd(),
    promptTemplateFactory: new HandlebarsPromptTemplateFactory()
);

kernel.Plugins.AddFromFunctions("TSqlTranslator", [nl2TsqlTranslator]);

Terminal.PrintAssistantMessage("Hello! I am an assistant that let's you use natural language to query the Northwind Sql Server database. How can I be of use to you?");

while (true)
{
    string request = Terminal.GetUserInput();

    var generatedQueryStream = kernel
                                .InvokeStreamingAsync(pluginName: "TSqlTranslator",
                                                      functionName: "nl2TsqlTranslator",
                                                      arguments: new() { { "request", request },
                                                                         { "tableDefinitions", TableProvider.GetTableDefinitions()} });

    string generatedQuery = await Terminal.GenerateTsqlQuery(generatedQueryStream);

    var queryResult = kernel.GetRequiredService<QueryExecutor>()
                            .ExecuteQuery(generatedQuery);

    Terminal.PrintTable(queryResult);
    Terminal.PrintAssistantMessage("Can I help you with anything else?");
}
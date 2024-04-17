using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using PlannerDemo.Plugins;
using System.Numerics;
using Microsoft.SemanticKernel.Planning.Handlebars;

IConfigurationRoot config = new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .Build();

AzureKeyCredential azureKeyCredential = new(config["AzureOpenAI:AzureKeyCredential"]);
string deploymentName = config["AzureOpenAI:DeploymentName"];
Uri endpoint = new Uri(config["AzureOpenAI:Endpoint"]);
OpenAIClient openAIClient = new(endpoint, azureKeyCredential);

IKernelBuilder builder = Kernel.CreateBuilder();

//builder.Services.AddLogging((options) =>
//{
//  options.SetMinimumLevel(LogLevel.Trace);
//  options.AddConsole();
//});

builder.Services.AddAzureOpenAIChatCompletion(deploymentName, openAIClient);

Kernel kernelWithMath = builder.Build();

kernelWithMath.Plugins.AddFromType<MathPlugin>();

HandlebarsPlanner planner = new HandlebarsPlanner(new HandlebarsPlannerOptions() { AllowLoops = true });

string userRequest = """
I currently have 13678.98 euros in my bank account. I am going to spend one third 
of that on my new garden shed. The rest I will invest at an annual interest rate of 5 percent
for the coming 10 years. How much money will I have after these 10 years?
""";
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine(userRequest);
Console.ForegroundColor = ConsoleColor.Cyan;
// Create a plan
var plan = await planner.CreatePlanAsync(kernelWithMath, userRequest);

Console.WriteLine($"The generated plan is:\n{plan}");
Console.ReadLine();
// Execute the plan
var result = (await plan.InvokeAsync(kernelWithMath, [])).Trim();
Console.WriteLine($"The generated result is:\n{result}");

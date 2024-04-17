using Azure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2SQL.Terminal.Configuration;
public static class SetUp
{
    public static (IConfiguration config, AzureKeyCredential? azureKeyCredential, string? deploymentName, Uri? endpoint) LoadConfiguration()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        AzureKeyCredential? azureKeyCredential = new(config["AzureOpenAI:AzureKeyCredential"]);
        string? deploymentName = config["AzureOpenAI:DeploymentName"];
        Uri? endpoint = new Uri(config["AzureOpenAI:Endpoint"]);

        return (config, azureKeyCredential, deploymentName, endpoint);
    }
}

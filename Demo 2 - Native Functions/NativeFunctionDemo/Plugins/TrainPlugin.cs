using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace NativeFunctionDemo.Plugins;
internal class TrainPlugin
{
  [KernelFunction, Description("Retrieve information about current disturbances with the Belgian Train traffic")]
  public async Task<string> GetDisturbanceInfo()
  {
    HttpClient client = new() { BaseAddress = new Uri("http://api.irail.be") };
    HttpResponseMessage response = await client.GetAsync("/disturbances/?format=json&lineBreakCharacter=''&lang=en");
    string disturbanceInfo = await response.Content.ReadAsStringAsync();

    return disturbanceInfo is null ? "Something went wrong while contacting external API" : disturbanceInfo;
  }
}

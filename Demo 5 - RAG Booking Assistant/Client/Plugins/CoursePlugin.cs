using HtmlAgilityPack;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text.Json;
using DTOs;
using System.Text.Json.Nodes;

namespace Client.Plugins;
public class CoursePlugin
{
    [KernelFunction]
    [Description("Retrieves the summary of a U2U course page coming from the U2U website https://www.u2u.be.")]
    public async Task<string> GetCoursePageSummary(
        Kernel kernel,
        [Description("A 5 to 6 character long string representing a U2U course. Some examples include 'UNASP', UADAI, 'UCSPR', \"UNOOP\"")] string courseCode
    )
    {
        string coursePageContent = await GetCourseContent(courseCode);
        /// Prompt the LLM to generate a list of steps to complete the task
        var result = await kernel.InvokePromptAsync($"""
        You are going to summarize the following information describing the contents of a U2U course. This summary should be one paragraph long:
        ***********
        {coursePageContent}
        """, new() {
            { "coursePageContent", coursePageContent }
        });

        // Return the plan back to the agent
        return result.ToString();
    }

    [KernelFunction]
    [Description("Requests the available dates that a U2U course takes place.")]
    public async Task<string> RequestBookingDatesAsync(
        Kernel kernel,
        [Description("A 5 to 6 character long string representing a U2U course. Some examples include 'UNASP', UADAI, 'UCSPR', \"UNOOP\"")] string courseCode
    )
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri("https://localhost:7130/");
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.GetAsync($"api/bookings/{courseCode}");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStreamAsync();
                var dates = await JsonSerializer.DeserializeAsync<List<DateTime>>(jsonResponse);
                string message = $"The available dates for course {courseCode} are";
                foreach(var date in dates)
                {
                    message += $"\n \t + {date}";
                }
                message += "\nPlease select one of these dates, along with your first and last name to make a booking.";
                return message;
            }
        }
        // Return the message back to the agent
        return "Unable to find dates for given request";
    }

    [KernelFunction]
    [Description("Makes a booking for a U2U course.")]
    public async Task<string> MakeBookingAsync(
        Kernel kernel,
        [Description("A 5 to 6 character long string representing a U2U course. Some examples include 'UNASP', UADAI, 'UCSPR', \"UNOOP\"")] string courseCode,
        [Description("The first name")] string firstName,
        [Description("The last name")] string lastName,
        [Description("The date the course will take place on")] DateTime date
    )
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri("https://localhost:7130/");
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            MakeBookingDTO makeBookingDTO = new MakeBookingDTO() 
            { 
                FirstName = firstName,
                LastName = lastName,
                CourseDate = date,
                CourseCode = courseCode
            };

            string jsonBody = JsonSerializer.Serialize(makeBookingDTO, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var body = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"api/bookings", body);
            if (response.IsSuccessStatusCode)
            {
                return "Your booking has been processed succesfully!";
            }
        }
        // Return the message back to the agent
        return "Something went wrong!";
    }


    private async Task<string> GetCourseContent(string courseCode)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, $"https://www.u2u.be/cc/{courseCode}");
            var response = await httpClient.SendAsync(message);
            string htmlDocument = await response.Content.ReadAsStringAsync();
            string body = GetBodyContent(htmlDocument);
            return body;
        }
    }

    private string GetBodyContent(string htmlDocument)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlDocument);

        // Select the body node and get its inner HTML
        HtmlNode bodyNode = doc.DocumentNode.SelectSingleNode("//div[@id='Outline']"); ;

        if (bodyNode != null)
        {
            string bodyContent = bodyNode.InnerHtml;
            return bodyContent;
        }

        // If no body node is found, you may want to handle this case accordingly
        return string.Empty;
    }
}

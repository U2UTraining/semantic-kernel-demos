using ConsoleTables;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2SQL.Terminal.Infra;
internal static class Terminal
{
    internal static void PrintAssistantMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Assistant > {message}");
        Console.ForegroundColor = ConsoleColor.White;
    }

    internal static string GetUserInput()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("User > ");
        string userInput = Console.ReadLine()!;
        Console.ForegroundColor = ConsoleColor.White;
        return userInput;
    }

    internal static async Task<string> GenerateTsqlQuery(IAsyncEnumerable<StreamingKernelContent> queryStream, bool print = true)
    {
        if (print)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("++++++++ The Generated T-SQL Query ++++++++\n");
        }

        string generatedQuery = string.Empty;
        await foreach (var content in queryStream)
        {
            if (print) Console.Write(content);
            generatedQuery += content.ToString();
        }

        if (print)
        {
            Console.WriteLine("\n\n+++++++++++++++++++++++++++++++++++++++++++");
            Console.ForegroundColor = ConsoleColor.White;
        }

        return generatedQuery!;
    }

    internal static void PrintTable(IEnumerable<dynamic> queryResult)
    {
        IDictionary<string, object> propertyValues = (IDictionary<string, object>)queryResult.First();
        string[] propertyNames = propertyValues.Keys.ToArray();

        var table = new ConsoleTable(propertyNames);

        foreach (var item in queryResult)
        {
            IDictionary<string, object> dict = (IDictionary<string, object>)item;

            var row = dict.Values.Select(value => value?.ToString()).ToArray();
            table.AddRow(row);
        }
        table.Write();
    }
}

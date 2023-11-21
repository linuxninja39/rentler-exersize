using System.CommandLine;
using System.CommandLine.Parsing;
using System.Text.Json;
using Rentler.Interview.Lib;
using Rentler.Interview.Lib.Models;
using static System.Console;

namespace Rentler.Interview.App;

class Program
{
    private static readonly Option<Uri> HostOption = new(
        name: "--host",
        description: "Host to connect to.",
        getDefaultValue: () => new Uri("https://localhost:7263")
    ) { IsRequired = true };

    private static readonly Option<Foods?> FoodOption = new(
        name: "--food",
        description: "Food",
        parseArgument: result =>
        {
            var val = result.Tokens.Single().Value ?? "{}";
            var food = JsonSerializer.Deserialize<Foods>(val);
            return food;
        }
    ) { IsRequired = true };

    private static readonly Option<int> PageOption = new(
        name: "--page",
        description: "Page",
        getDefaultValue: () => 0
    );

    private static readonly Option<int> SizeOption = new(
        name: "--size",
        description: "Size",
        getDefaultValue: () => 3
    );


    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Sample app for Foods service");
        rootCommand.AddOption(HostOption);
        AddCommand(rootCommand);
        SearchCommand(rootCommand);
        GetAllCommand(rootCommand);
        DeleteCommand(rootCommand);
        UpdateCommand(rootCommand);

        return await rootCommand.InvokeAsync(args);
    }

    private static void AddCommand(RootCommand rootCommand)
    {
        var addCommand = new Command("add", "Add a new food")
        {
            FoodOption
        };

        addCommand.SetHandler(async (food, host) =>
        {
            var client = new FoodsClient(host);
            var newFood = await client.AddFood(food);
            WriteLine($"Added food: {JsonSerializer.Serialize(newFood)}");
        }, FoodOption, HostOption);

        rootCommand.AddCommand(addCommand);
    }

    private static void SearchCommand(RootCommand rootCommand)
    {
        Option<string> searchTermOption = new(
            name: "--searchTerm",
            description: "Search Term"
        ) { IsRequired = true };

        var command = new Command("search", "Search")
        {
            PageOption,
            SizeOption,
            searchTermOption
        };

        command.SetHandler(async (host, page, size, searchTerm) =>
        {
            var client = new FoodsClient(host);
            var ret = await client.SearchFoods(page, size, searchTerm);
            WriteLine($"Search response: {JsonSerializer.Serialize(ret)}");
        }, HostOption, PageOption, SizeOption, searchTermOption);

        rootCommand.AddCommand(command);
    }

    private static void GetAllCommand(RootCommand rootCommand)
    {
        var command = new Command("get", "Get all")
        {
            PageOption,
            SizeOption
        };

        command.SetHandler(async (host, page, size) =>
        {
            var client = new FoodsClient(host);
            var ret = await client.GetAllFoods(page, size);
            WriteLine($"Get All response: {JsonSerializer.Serialize(ret)}");
        }, HostOption, PageOption, SizeOption);

        rootCommand.AddCommand(command);
    }

    private static void UpdateCommand(RootCommand rootCommand)
    {
        var command = new Command("update", "Update")
        {
            FoodOption,
        };

        command.SetHandler(async (host, food) =>
        {
            var client = new FoodsClient(host);
            var ret = await client.UpdateFood(food);
            WriteLine(
                $"Food updated: {JsonSerializer.Serialize(ret, new JsonSerializerOptions { PropertyNameCaseInsensitive = false })}"
            );
        }, HostOption, FoodOption);

        rootCommand.AddCommand(command);
    }

    private static void DeleteCommand(RootCommand rootCommand)
    {
        Option<int> deleteOption = new(
            name: "--foodId",
            description: "Food Id"
        ) { IsRequired = true };

        var command = new Command("delete", "Delete")
        {
            deleteOption,
        };

        command.SetHandler(async (host, foodId) =>
        {
            var client = new FoodsClient(host);
            await client.DeleteFood(foodId);
            WriteLine(
                $"Food deleted: {foodId}"
            );
        }, HostOption, deleteOption);

        rootCommand.AddCommand(command);
    }
}
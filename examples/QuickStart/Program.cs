using Trix;
using Trix.Models;

Console.WriteLine("Trix SDK for .NET - Quick Start Example");
Console.WriteLine("========================================");

// Check for API key
var apiKey = Environment.GetEnvironmentVariable("TRIX_API_KEY");
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine();
    Console.WriteLine("To run this example, set the TRIX_API_KEY environment variable:");
    Console.WriteLine("  export TRIX_API_KEY=your_api_key");
    Console.WriteLine();
    Console.WriteLine("Showing SDK usage examples (not connected to API)...");
    Console.WriteLine();
    ShowExamples();
    return;
}

// Create client from environment
using var client = TrixClient.FromEnvironment();

Console.WriteLine($"Connected to Trix (SDK v{TrixClient.Version})");
Console.WriteLine();

try
{
    // Create a memory
    Console.WriteLine("Creating a memory...");
    var memory = await client.Memories.CreateAsync(new CreateMemoryRequest
    {
        Content = "The Trix SDK for .NET provides a type-safe way to interact with the Trix API.",
        Type = MemoryType.Text,
        Tags = new List<string> { "sdk", "dotnet", "example" }
    });
    Console.WriteLine($"Created memory: {memory.Id}");
    Console.WriteLine($"  Content: {memory.Content}");
    Console.WriteLine();

    // List memories
    Console.WriteLine("Listing memories...");
    var memories = await client.Memories.ListAsync(new ListMemoriesRequest
    {
        Limit = 5,
        SortBy = "createdAt",
        SortOrder = "desc"
    });
    Console.WriteLine($"Found {memories.Data.Count} memories:");
    foreach (var m in memories.Data)
    {
        Console.WriteLine($"  - [{m.Id}] {m.Content[..Math.Min(50, m.Content.Length)]}...");
    }
    Console.WriteLine();

    // Create a relationship (if we have multiple memories)
    if (memories.Data.Count >= 2)
    {
        Console.WriteLine("Creating a relationship...");
        var relationship = await client.Relationships.CreateAsync(
            sourceId: memories.Data[0].Id,
            targetId: memories.Data[1].Id,
            relationshipType: RelationshipTypes.RelatedTo,
            weight: 0.8
        );
        Console.WriteLine($"Created relationship: {relationship.Id}");
        Console.WriteLine($"  {relationship.SourceId} --[{relationship.RelationshipType}]--> {relationship.TargetId}");
        Console.WriteLine();
    }

    // Clean up - delete the memory we created
    Console.WriteLine("Cleaning up...");
    await client.Memories.DeleteAsync(memory.Id);
    Console.WriteLine($"Deleted memory: {memory.Id}");
}
catch (Trix.Exceptions.TrixException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"  Status: {ex.StatusCode}");
    Console.WriteLine($"  Request ID: {ex.RequestId}");
}

Console.WriteLine();
Console.WriteLine("Done!");

static void ShowExamples()
{
    Console.WriteLine("Example 1: Create a client");
    Console.WriteLine(@"
    using var client = new TrixClient(""your_api_key"");
    // or
    using var client = TrixClient.FromEnvironment();
");

    Console.WriteLine("Example 2: Create a memory");
    Console.WriteLine(@"
    var memory = await client.Memories.CreateAsync(new CreateMemoryRequest
    {
        Content = ""Hello, Trix!"",
        Type = MemoryType.Text,
        Tags = new List<string> { ""greeting"" }
    });
");

    Console.WriteLine("Example 3: Search memories");
    Console.WriteLine(@"
    var results = await client.Memories.ListAsync(new ListMemoriesRequest
    {
        Q = ""search query"",
        Mode = SearchMode.Semantic,
        Limit = 10
    });
");

    Console.WriteLine("Example 4: Create a relationship");
    Console.WriteLine(@"
    var relationship = await client.Relationships.CreateAsync(
        sourceId: ""mem_1"",
        targetId: ""mem_2"",
        relationshipType: RelationshipTypes.RelatedTo
    );
");

    Console.WriteLine("Example 5: Handle errors");
    Console.WriteLine(@"
    try
    {
        var memory = await client.Memories.GetAsync(""mem_invalid"");
    }
    catch (NotFoundException ex)
    {
        Console.WriteLine($""Not found: {ex.Message}"");
    }
");
}

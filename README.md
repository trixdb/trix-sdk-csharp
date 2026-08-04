# Trix SDK for .NET

The official .NET SDK for [Trix](https://trixdb.com) - a memory and knowledge management API.

[![NuGet](https://img.shields.io/nuget/v/Trix.svg)](https://www.nuget.org/packages/Trix)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%2010.0-blue.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

## Installation

Install via NuGet:

```bash
dotnet add package Trix
```

Or via the Package Manager Console:

```powershell
Install-Package Trix
```

## Quick Start

```csharp
using Trix;
using Trix.Models;

// Create client with API key
using var client = new TrixClient("your_api_key");

// Or from environment variables (TRIX_API_KEY)
using var client = TrixClient.FromEnvironment();

// Create a memory
var memory = await client.Memories.CreateAsync(new CreateMemoryRequest
{
    Content = "Trix is a powerful memory and knowledge management API.",
    Type = MemoryType.Text,
    Tags = new List<string> { "introduction", "trix" }
});

Console.WriteLine($"Created memory: {memory.Id}");

// Search memories
var results = await client.Memories.ListAsync(new ListMemoriesRequest
{
    Q = "memory management",
    Mode = SearchMode.Semantic,
    Limit = 10
});

foreach (var m in results.Data)
{
    Console.WriteLine($"- {m.Content}");
}
```

## Features

- **Memories**: Create, read, update, delete, and search memories
- **Relationships**: Connect memories with typed relationships
- **Clusters**: Group related memories into clusters
- **Spaces**: Organize memories into workspaces
- **Facts & Entities**: Knowledge graph with facts and named entities
- **Async/Await**: Full async support with cancellation tokens
- **Resilience**: Built-in retry with exponential backoff
- **Type Safety**: Strongly typed request/response models

## Configuration

### Basic Configuration

```csharp
var options = new TrixClientOptions
{
    ApiKey = "your_api_key",
    BaseUrl = "https://api.trixdb.com",  // Default
    Timeout = TimeSpan.FromSeconds(30),   // Default
    MaxRetries = 3                         // Default
};

using var client = new TrixClient(options);
```

### Environment Variables

```csharp
// Reads from TRIX_API_KEY and optionally TRIX_BASE_URL
using var client = TrixClient.FromEnvironment();
```

### Dependency Injection

```csharp
// In Startup.cs or Program.cs
services.AddSingleton(sp =>
{
    var options = new TrixClientOptions
    {
        ApiKey = configuration["Trix:ApiKey"]
    };
    return new TrixClient(options);
});
```

## Resources

### Memories

```csharp
// Create
var memory = await client.Memories.CreateAsync(new CreateMemoryRequest
{
    Content = "Hello, Trix!",
    Type = MemoryType.Text,
    Tags = new List<string> { "greeting" }
});

// Get
var memory = await client.Memories.GetAsync("mem_123");

// Update
var updated = await client.Memories.UpdateAsync("mem_123", new UpdateMemoryRequest
{
    Content = "Updated content",
    Tags = new List<string> { "updated" }
});

// Delete
await client.Memories.DeleteAsync("mem_123");

// List with filters
var memories = await client.Memories.ListAsync(new ListMemoriesRequest
{
    Q = "search query",
    Mode = SearchMode.Semantic,
    Tags = new List<string> { "tag1" },
    Limit = 20
});

// Iterate all matching memories
await foreach (var memory in client.Memories.ListAllAsync())
{
    Console.WriteLine(memory.Content);
}
```

### Relationships

```csharp
// Create relationship
var relationship = await client.Relationships.CreateAsync(
    sourceId: "mem_1",
    targetId: "mem_2",
    relationshipType: RelationshipTypes.RelatedTo,
    weight: 0.8
);

// Reinforce a relationship
await client.Relationships.ReinforceAsync("rel_123", amount: 0.1);

// Weaken a relationship
await client.Relationships.WeakenAsync("rel_123", amount: 0.1);

// Get relationships for a memory
var incoming = await client.Relationships.GetIncomingAsync("mem_123");
var outgoing = await client.Relationships.GetOutgoingAsync("mem_123");
```

### Clusters

```csharp
// Create cluster
var cluster = await client.Clusters.CreateAsync("My Cluster",
    description: "A collection of related memories");

// Add memory to cluster
await client.Clusters.AddMemoryAsync("cluster_123", "mem_456");

// Expand cluster with similar memories
var expansion = await client.Clusters.ExpandAsync("cluster_123",
    limit: 10,
    threshold: 0.7);
```

### Spaces

```csharp
// Create space
var space = await client.Spaces.CreateAsync("My Workspace",
    description: "A workspace for my project");

// List spaces
var spaces = await client.Spaces.ListAsync();

// Create memory in a space
var memory = await client.Memories.CreateAsync(new CreateMemoryRequest
{
    Content = "Memory in specific space",
    SpaceId = space.Id
});
```

## Error Handling

The SDK throws specific exceptions for different error types:

```csharp
try
{
    var memory = await client.Memories.GetAsync("mem_invalid");
}
catch (NotFoundException ex)
{
    Console.WriteLine($"Memory not found: {ex.Message}");
}
catch (AuthenticationException ex)
{
    Console.WriteLine($"Invalid API key: {ex.Message}");
}
catch (RateLimitException ex)
{
    Console.WriteLine($"Rate limited. Retry after {ex.RetryAfterSeconds}s");
}
catch (ValidationException ex)
{
    Console.WriteLine($"Validation failed: {ex.Message}");
    foreach (var error in ex.Errors ?? new Dictionary<string, string[]>())
    {
        Console.WriteLine($"  {error.Key}: {string.Join(", ", error.Value)}");
    }
}
catch (TrixException ex)
{
    Console.WriteLine($"Trix error: {ex.Message}");
    Console.WriteLine($"  Status: {ex.StatusCode}");
    Console.WriteLine($"  Request ID: {ex.RequestId}");
}
```

## Cancellation

All async methods support cancellation tokens:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

try
{
    var memories = await client.Memories.ListAsync(cancellationToken: cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation was cancelled");
}
```

## Logging

Configure logging via the options:

```csharp
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder =>
    builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

var options = new TrixClientOptions
{
    ApiKey = "your_api_key",
    LoggerFactory = loggerFactory
};

using var client = new TrixClient(options);
```

## Requirements

- .NET 8.0 or later — the package multi-targets `net8.0` and `net10.0`.

## Related SDKs

- [Python SDK](https://github.com/trixdb/trix-sdk-python)
- [TypeScript SDK](https://github.com/trixdb/trix-sdk-typescript)

## License

MIT License - see [LICENSE](LICENSE) for details.

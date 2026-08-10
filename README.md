# Trix SDK for .NET

The official .NET SDK for [Trix](https://trixdb.com) - a memory and knowledge management API.

[![NuGet](https://img.shields.io/nuget/v/Trix.svg)](https://www.nuget.org/packages/Trix)
[![Version](https://img.shields.io/badge/version-0.6.0-blue.svg)](https://www.nuget.org/packages/Trix/0.6.0)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%2010.0-blue.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-Apache_2.0-blue.svg)](LICENSE)

## Features

- **Typed & async** - strongly typed request/response models; every call is `async`/`await` and accepts a `CancellationToken`.
- **Broad coverage** - 22 resource namespaces including Memories, Relationships, Clusters, Spaces, Facts, Entities, Graph, Search, Tasks, Webhooks, Jobs, and Highlights.
- **Automatic pagination** - `ListAllAsync(...)` returns `IAsyncEnumerable<T>` and walks pages transparently.
- **Automatic retries** - exponential backoff with jitter; honors `Retry-After` on HTTP 429.
- **Idempotency** - mutating requests carry a stable `Idempotency-Key` reused across retries, so a committed-but-failed write is never duplicated.
- **Webhooks** - full management API plus inbound **signature verification** (HMAC-SHA256, constant-time, replay-protected).
- **File uploads** - multipart image upload and binary download.
- **Typed errors** - a `TrixException` hierarchy carrying `StatusCode`, `ErrorCode`, and `RequestId`.
- **Secure by default** - HTTPS enforced and an SSRF guard blocks private/loopback/link-local base URLs.
- **DI-friendly** - thread-safe client usable as a singleton, with a pluggable `HttpMessageHandler` and `ILoggerFactory`.

> **Not yet available:** token streaming (SSE) and a shipped mock/test double are on the roadmap. For a streaming-first client today, see the [Go SDK](https://github.com/trixdb/trix-sdk-go).

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

## Authentication

```csharp
// 1. API key via constructor
using var client = new TrixClient("your_api_key");

// 2. From environment - reads TRIX_API_KEY and optionally TRIX_BASE_URL
using var client = TrixClient.FromEnvironment();

// 3. Explicit options - supply an API key or a JWT token
using var client = new TrixClient(new TrixClientOptions
{
    ApiKey = "your_api_key"   // or: JwtToken = "eyJ..."
});
```

The credential is attached to every request by the SDK. `Authorization` is a restricted header and cannot be overridden through `CustomHeaders`.

## Configuration

```csharp
var options = new TrixClientOptions
{
    ApiKey = "your_api_key",
    BaseUrl = "https://api.trixdb.com",       // default
    Timeout = TimeSpan.FromSeconds(30),        // default (max 1 hour)
    MaxRetries = 3,                            // default (max 10)
    CustomHeaders = new Dictionary<string, string>
    {
        ["X-Tenant"] = "acme"
    },
    HttpHandler = new SocketsHttpHandler()     // bring your own handler (proxy, pooling, mTLS)
};

using var client = new TrixClient(options);
```

- **`BaseUrl`** must use HTTPS. Private, loopback, and link-local hosts are rejected by the SSRF guard; set `AllowInsecure = true` only for local development.
- **`CustomHeaders`** are attached to every request; security-sensitive headers (`Authorization`, `Host`, `Content-Length`, ...) cannot be set here.
- **`HttpHandler`** supplies a custom `HttpMessageHandler`; a handler you own is left for you to dispose.

### Environment Variables

```csharp
// Reads from TRIX_API_KEY and optionally TRIX_BASE_URL
using var client = TrixClient.FromEnvironment();
```

### Dependency Injection

```csharp
// In Startup.cs or Program.cs
services.AddSingleton(sp => new TrixClient(new TrixClientOptions
{
    ApiKey = configuration["Trix:ApiKey"]
}));
```

## Resources

The client exposes resource namespaces including `Memories`, `Relationships`, `Clusters`, `Spaces`, `Facts`, `Entities`, `Graph`, `Search`, `Tasks`, and `Webhooks`.

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

### Tasks

```csharp
// Create a task
var task = await client.Tasks.CreateAsync(new CreateTaskRequest
{
    Title = "Ship the C# SDK",
    SpaceId = space.Id,
    Priority = 1,                      // 1 = highest .. 5 = lowest
    DueAt = DateTimeOffset.UtcNow.AddDays(7)
});

// List (requires SpaceId or AssigneeId)
var page = await client.Tasks.ListAsync(new ListTasksRequest
{
    SpaceId = space.Id,
    Status = TaskStatuses.Todo
});
Console.WriteLine($"{page.Tasks.Count} of {page.Total} (more: {page.HasMore})");

// Iterate every matching task across pages
await foreach (var t in client.Tasks.ListAllAsync(new ListTasksRequest { SpaceId = space.Id }))
{
    Console.WriteLine(t.Title);
}

// Update with optimistic concurrency, then complete
await client.Tasks.UpdateAsync(task.Id, new UpdateTaskRequest
{
    Description = "Publish to NuGet",
    Version = task.Version
});
await client.Tasks.CompleteAsync(task.Id);   // sets status = done

// Subtasks
var sub = await client.Tasks.CreateSubtaskAsync(task.Id, new CreateSubtaskRequest { Title = "Write README" });
var subtasks = await client.Tasks.GetSubtasksAsync(task.Id);
```

## Pagination

List endpoints return a `PaginatedResponse<T>` - the `Data` items plus a `Pagination` block (`Total`, `Page`, `Limit`, `HasMore`):

```csharp
var page = await client.Memories.ListAsync(new ListMemoriesRequest { Limit = 50 });
Console.WriteLine($"{page.Data.Count} of {page.Pagination?.Total} (more: {page.Pagination?.HasMore})");
```

To iterate every matching item without managing page numbers, use `ListAllAsync`, which returns an `IAsyncEnumerable<T>` and fetches each page on demand:

```csharp
await foreach (var memory in client.Memories.ListAllAsync(new ListMemoriesRequest { Q = "trix" }))
{
    Console.WriteLine(memory.Content);
}
```

`ListAllAsync` is available on Memories, Facts, Entities, Highlights, Jobs, Clusters, Tasks, and Webhooks. It observes the `CancellationToken` and stops as soon as `HasMore` is false.

## File Uploads

Create image memories with a multipart upload (from a `Stream` or `byte[]`), then download the stored image back as a stream or bytes:

```csharp
// Upload from a file stream
using var imageStream = File.OpenRead("photo.jpg");
var memory = await client.Memories.CreateFromImageAsync(
    imageStream,
    "photo.jpg",
    "image/jpeg",
    new CreateImageMemoryRequest { AutoTag = true });

// Download the original image bytes
byte[] bytes = await client.Memories.GetImageBytesAsync(memory.Id);
await File.WriteAllBytesAsync("downloaded.jpg", bytes);
```

Supported formats: jpg, jpeg, png, gif, webp, bmp, tiff. `GetImageAsync` returns a `Stream` if you prefer to copy directly, and `GetThumbnailAsync` fetches a generated thumbnail.

## Idempotency

Every mutating request (POST/PUT/PATCH/DELETE, including multipart uploads) automatically carries an `Idempotency-Key`. The SDK generates one key per logical request and reuses it across the whole retry sequence, so a write that commits server-side but then times out or returns 5xx is retried safely without creating a duplicate. Read requests send no key. This is automatic - there is nothing to configure.

## Webhooks

Manage webhook endpoints through `client.Webhooks`:

```csharp
var hook = await client.Webhooks.CreateAsync(new CreateWebhookRequest
{
    Url = "https://example.com/hooks/trix",
    Events = new List<string> { "memory.created" }
});

// Send a test event, then list every endpoint across pages
await client.Webhooks.TestAsync(hook.Id);
await foreach (var w in client.Webhooks.ListAllAsync())
{
    Console.WriteLine($"{w.Id} -> {w.Url}");
}
```

### Verifying inbound webhooks

`VerifySignature` and `Unwrap<T>` are **static** helpers on `WebhooksResource`, so a receiver can verify a request without constructing a client. They recompute an HMAC-SHA256 over `{timestamp}.{payload}` keyed by the endpoint's signing secret, compare it against the `X-Webhook-Signature` header (`t=...,v1=...`) in constant time via `CryptographicOperations.FixedTimeEquals`, and reject anything older than the tolerance (default **300s**) to stop replays. Always pass the **raw** request body - re-serializing it changes the bytes and fails verification.

```csharp
using Trix.Resources;
using Trix.Exceptions;

// ASP.NET Core Minimal API endpoint
app.MapPost("/hooks/trix", async (HttpRequest req) =>
{
    var raw = await new StreamReader(req.Body).ReadToEndAsync();
    var signature = req.Headers["X-Webhook-Signature"].ToString();

    // Boolean check - fails closed: returns false for a missing, malformed,
    // expired, or tampered signature (never throws for a bad signature).
    if (!WebhooksResource.VerifySignature(raw, signature, mySigningSecret))
        return Results.Unauthorized();

    return Results.Ok();
});
```

Prefer `Unwrap<T>` when you want the verified event typed in one step - it verifies, then deserializes, and throws `TrixWebhookVerificationException` on any failure (bad signature, replay, or a body that will not deserialize):

```csharp
using System.Text.Json;
using Trix.Resources;
using Trix.Exceptions;

try
{
    // T is your own event shape; JsonElement works when you want the raw tree.
    var evt = WebhooksResource.Unwrap<JsonElement>(raw, signature, mySigningSecret);
    Console.WriteLine(evt.GetProperty("event").GetString());
}
catch (TrixWebhookVerificationException)
{
    // Treat as untrusted - respond 400/401 and do not process.
}
```

A non-default replay window is available through the `toleranceSeconds` parameter on `VerifySignature`.

## Error Handling

The SDK throws specific exceptions for different error types. All derive from `TrixException`, which carries `StatusCode`, `ErrorCode`, and `RequestId`:

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

| Exception | Raised for |
|-----------|-----------|
| `AuthenticationException` | 401 - missing or invalid credentials |
| `PermissionException` | 403 - authenticated but not allowed |
| `NotFoundException` | 404 - resource does not exist |
| `ValidationException` | Invalid request; per-field detail in `Errors` |
| `RateLimitException` | 429 - includes `RetryAfterSeconds` |
| `ServerException` | 5xx - server-side failure |
| `NetworkException` | Transport/connection failure |
| `TrixTimeoutException` | Request exceeded the configured timeout |
| `TrixWebhookVerificationException` | Inbound webhook failed signature verification |

## Automatic Retries

The client retries transient failures automatically: network errors, timeouts, HTTP 429, and 5xx responses. Other 4xx client errors are never retried. Backoff is exponential (1s base, capped at 30s) with 0-30% jitter; on a 429 the `Retry-After` value is honored before backing off. Tune the attempt count with `MaxRetries` (default 3, max 10):

```csharp
using var client = new TrixClient(new TrixClientOptions
{
    ApiKey = "your_api_key",
    MaxRetries = 5
});
```

Because retries reuse a single `Idempotency-Key` per request (see [Idempotency](#idempotency)), a retried write is safe against duplication.

## Cancellation

All async methods accept a cancellation token:

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

- .NET 8.0 or later - the package multi-targets `net8.0` and `net10.0`.
- Nullable reference types are enabled throughout the public surface.

## Related SDKs

- [Python SDK](https://github.com/trixdb/trix-sdk-python)
- [TypeScript SDK](https://github.com/trixdb/trix-sdk-typescript)
- [Go SDK](https://github.com/trixdb/trix-sdk-go) - streaming-focused client
- .NET SDK (this repository)

## License

Apache License 2.0 - see [LICENSE](LICENSE) for details.

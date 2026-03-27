using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Trix.Internal;
using Trix.Resources;

namespace Trix;

/// <summary>
/// The main Trix client for interacting with the Trix API.
/// </summary>
/// <remarks>
/// This client is thread-safe and can be used as a singleton.
/// It implements <see cref="IDisposable"/> to clean up resources.
/// </remarks>
/// <example>
/// Using API key:
/// <code>
/// using var client = new TrixClient("your_api_key");
/// var memory = await client.Memories.CreateAsync(new CreateMemoryRequest { Content = "Hello, Trix!" });
/// </code>
/// </example>
public sealed class TrixClient : IDisposable
{
    private readonly HttpPipeline _pipeline;
    private readonly ILogger<TrixClient> _logger;
    private int _disposed;

    /// <summary>
    /// Gets the SDK version.
    /// </summary>
    public static string Version => HttpPipeline.SdkVersion;

    /// <summary>
    /// Gets the API version.
    /// </summary>
    public static string ApiVersion => HttpPipeline.ApiVersion;

    // Private backing fields for resources
    private readonly MemoriesResource _memories;
    private readonly RelationshipsResource _relationships;
    private readonly ClustersResource _clusters;
    private readonly SpacesResource _spaces;
    private readonly GraphResource _graph;
    private readonly SearchResource _search;
    private readonly WebhooksResource _webhooks;
    private readonly AgentResource _agent;
    private readonly FeedbackResource _feedback;
    private readonly HighlightsResource _highlights;
    private readonly JobsResource _jobs;
    private readonly FactsResource _facts;
    private readonly EntitiesResource _entities;
    private readonly EnrichmentsResource _enrichments;
    private readonly SessionsResource _sessions;
    private readonly ResourcesResource _resources;
    private readonly SpaceConfigResource _spaceConfig;
    private readonly HubRolesResource _hubRoles;
    private readonly CalendarResource _calendar;

    /// <summary>
    /// Gets the memories resource for managing memories.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public MemoriesResource Memories { get { ThrowIfDisposed(); return _memories; } }

    /// <summary>
    /// Gets the relationships resource for managing relationships.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public RelationshipsResource Relationships { get { ThrowIfDisposed(); return _relationships; } }

    /// <summary>
    /// Gets the clusters resource for managing clusters.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public ClustersResource Clusters { get { ThrowIfDisposed(); return _clusters; } }

    /// <summary>
    /// Gets the spaces resource for managing spaces.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public SpacesResource Spaces { get { ThrowIfDisposed(); return _spaces; } }

    /// <summary>
    /// Gets the graph resource for knowledge graph traversal.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public GraphResource Graph { get { ThrowIfDisposed(); return _graph; } }

    /// <summary>
    /// Gets the search resource for semantic search operations.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public SearchResource Search { get { ThrowIfDisposed(); return _search; } }

    /// <summary>
    /// Gets the webhooks resource for managing webhooks.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public WebhooksResource Webhooks { get { ThrowIfDisposed(); return _webhooks; } }

    /// <summary>
    /// Gets the agent resource for agent sessions and core memory.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public AgentResource Agent { get { ThrowIfDisposed(); return _agent; } }

    /// <summary>
    /// Gets the feedback resource for submitting feedback.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public FeedbackResource Feedback { get { ThrowIfDisposed(); return _feedback; } }

    /// <summary>
    /// Gets the highlights resource for managing highlights.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public HighlightsResource Highlights { get { ThrowIfDisposed(); return _highlights; } }

    /// <summary>
    /// Gets the jobs resource for background job management.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public JobsResource Jobs { get { ThrowIfDisposed(); return _jobs; } }

    /// <summary>
    /// Gets the facts resource for knowledge graph facts.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public FactsResource Facts { get { ThrowIfDisposed(); return _facts; } }

    /// <summary>
    /// Gets the entities resource for named entity management.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public EntitiesResource Entities { get { ThrowIfDisposed(); return _entities; } }

    /// <summary>
    /// Gets the enrichments resource for memory enrichments.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public EnrichmentsResource Enrichments { get { ThrowIfDisposed(); return _enrichments; } }

    /// <summary>
    /// Gets the sessions resource for managing CLI sessions.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public SessionsResource Sessions { get { ThrowIfDisposed(); return _sessions; } }

    /// <summary>
    /// Gets the resources resource for managing resources (projects, topics, etc.).
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public ResourcesResource Resources { get { ThrowIfDisposed(); return _resources; } }

    /// <summary>
    /// Gets the space config resource for managing per-space configuration.
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public SpaceConfigResource SpaceConfig { get { ThrowIfDisposed(); return _spaceConfig; } }

    /// <summary>
    /// Gets the hub roles resource for managing custom roles (ADR-080).
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public HubRolesResource HubRoles { get { ThrowIfDisposed(); return _hubRoles; } }

    /// <summary>
    /// Gets the calendar resource for calendar operations (ADR-075).
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the client has been disposed.</exception>
    public CalendarResource Calendar { get { ThrowIfDisposed(); return _calendar; } }

    /// <summary>
    /// Creates a new Trix client with the specified API key.
    /// </summary>
    /// <param name="apiKey">The API key for authentication.</param>
    /// <param name="baseUrl">Optional custom base URL.</param>
    public TrixClient(string apiKey, string? baseUrl = null)
        : this(new TrixClientOptions
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? TrixClientOptions.DefaultBaseUrl
        })
    {
    }

    /// <summary>
    /// Creates a new Trix client with the specified options.
    /// </summary>
    /// <param name="options">The client configuration options.</param>
    /// <exception cref="ArgumentNullException">If options is null.</exception>
    /// <exception cref="ArgumentException">If options are invalid.</exception>
    public TrixClient(TrixClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();

        var loggerFactory = options.LoggerFactory ?? NullLoggerFactory.Instance;
        _logger = loggerFactory.CreateLogger<TrixClient>();

        _pipeline = new HttpPipeline(options);

        // Initialize resources
        _memories = new MemoriesResource(_pipeline);
        _relationships = new RelationshipsResource(_pipeline);
        _clusters = new ClustersResource(_pipeline);
        _spaces = new SpacesResource(_pipeline);
        _graph = new GraphResource(_pipeline);
        _search = new SearchResource(_pipeline);
        _webhooks = new WebhooksResource(_pipeline);
        _agent = new AgentResource(_pipeline);
        _feedback = new FeedbackResource(_pipeline);
        _highlights = new HighlightsResource(_pipeline);
        _jobs = new JobsResource(_pipeline);
        _facts = new FactsResource(_pipeline);
        _entities = new EntitiesResource(_pipeline);
        _enrichments = new EnrichmentsResource(_pipeline);
        _sessions = new SessionsResource(_pipeline);
        _resources = new ResourcesResource(_pipeline);
        _spaceConfig = new SpaceConfigResource(_pipeline);
        _hubRoles = new HubRolesResource(_pipeline);
        _calendar = new CalendarResource(_pipeline);

        _logger.LogInformation("TrixClient initialized (SDK v{Version})", Version);
    }

    /// <summary>
    /// Creates a Trix client from environment variables.
    /// Reads TRIX_API_KEY and optionally TRIX_BASE_URL.
    /// </summary>
    /// <returns>A configured Trix client.</returns>
    /// <exception cref="InvalidOperationException">If TRIX_API_KEY is not set.</exception>
    public static TrixClient FromEnvironment()
    {
        var options = TrixClientOptions.FromEnvironment();
        return new TrixClient(options);
    }

    /// <summary>
    /// Creates a Trix client from environment variables with custom options.
    /// </summary>
    /// <param name="configure">Action to configure additional options.</param>
    /// <returns>A configured Trix client.</returns>
    public static TrixClient FromEnvironment(Action<TrixClientOptions> configure)
    {
        var options = TrixClientOptions.FromEnvironment();
        configure(options);
        return new TrixClient(options);
    }

    /// <summary>
    /// Disposes the client and releases resources.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        _pipeline.Dispose();

        _logger.LogDebug("TrixClient disposed");
    }

    /// <summary>
    /// Throws if the client has been disposed.
    /// </summary>
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
    }
}

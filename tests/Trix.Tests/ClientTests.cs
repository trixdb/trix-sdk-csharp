using FluentAssertions;
using Trix;

namespace Trix.Tests;

public class ClientTests
{
    [Fact]
    public void Constructor_WithApiKey_CreatesClient()
    {
        // Arrange & Act
        using var client = new TrixClient("test_api_key", "https://api.test.com");

        // Assert
        client.Should().NotBeNull();

        // Core resources
        client.Memories.Should().NotBeNull();
        client.Relationships.Should().NotBeNull();
        client.Clusters.Should().NotBeNull();
        client.Spaces.Should().NotBeNull();

        // Knowledge graph resources
        client.Graph.Should().NotBeNull();
        client.Facts.Should().NotBeNull();
        client.Entities.Should().NotBeNull();

        // Additional resources
        client.Search.Should().NotBeNull();
        client.Webhooks.Should().NotBeNull();
        client.Agent.Should().NotBeNull();
        client.Feedback.Should().NotBeNull();
        client.Highlights.Should().NotBeNull();
        client.Jobs.Should().NotBeNull();
        client.Enrichments.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithOptions_CreatesClient()
    {
        // Arrange
        var options = new TrixClientOptions
        {
            ApiKey = "test_api_key",
            BaseUrl = "https://api.test.com",
            Timeout = TimeSpan.FromSeconds(60),
            MaxRetries = 5
        };

        // Act
        using var client = new TrixClient(options);

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullApiKey_ThrowsException()
    {
        // Arrange
        var options = new TrixClientOptions { BaseUrl = "https://api.test.com" };

        // Act & Assert
        var act = () => new TrixClient(options);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithHttpUrl_ThrowsException()
    {
        // Arrange
        var options = new TrixClientOptions
        {
            ApiKey = "test_api_key",
            BaseUrl = "http://api.test.com"
        };

        // Act & Assert
        var act = () => new TrixClient(options);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithHttpUrlAndAllowInsecure_Succeeds()
    {
        // Arrange
        var options = new TrixClientOptions
        {
            ApiKey = "test_api_key",
            BaseUrl = "http://localhost:8080",
            AllowInsecure = true
        };

        // Act
        using var client = new TrixClient(options);

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void Version_ReturnsVersion()
    {
        // Act
        var version = TrixClient.Version;

        // Assert
        version.Should().NotBeNullOrEmpty();
        // Single-sourced from the assembly version (csproj <Version>), not a literal.
        version.Should().Be(typeof(TrixClient).Assembly.GetName().Version!.ToString(3));
    }

    [Fact]
    public void ApiVersion_ReturnsApiVersion()
    {
        // Act
        var apiVersion = TrixClient.ApiVersion;

        // Assert
        apiVersion.Should().NotBeNullOrEmpty();
        apiVersion.Should().Be("v1");
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var client = new TrixClient("test_api_key", "https://api.test.com");

        // Act & Assert - should not throw
        client.Dispose();
        client.Dispose();
    }
}

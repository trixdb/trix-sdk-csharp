# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.6.0] - 2025-12-27

### Added

- Initial release of Trix SDK for .NET
- **TrixClient**: Main client for API interaction
  - Support for API key and JWT authentication
  - Configurable timeouts and retry policies
  - Environment variable configuration
- **MemoriesResource**: Full CRUD operations for memories
  - Create, get, update, delete memories
  - List with filters (search, tags, type, space)
  - Bulk create operations
  - Async enumerable for pagination
- **RelationshipsResource**: Relationship management
  - Create relationships between memories
  - Reinforce and weaken relationships
  - Get incoming/outgoing relationships
- **ClustersResource**: Cluster operations
  - Create and manage clusters
  - Add/remove memories from clusters
  - Cluster expansion suggestions
- **SpacesResource**: Workspace management
  - Create and manage spaces
- **Models**: Strongly typed models
  - Memory, Relationship, Cluster, Space
  - Fact, Entity for knowledge graph
  - Paginated responses
- **Exceptions**: Typed exception hierarchy
  - TrixException base class
  - AuthenticationException, PermissionException
  - NotFoundException, ValidationException
  - RateLimitException, ServerException
  - NetworkException, TrixTimeoutException
- **Resilience**: Built-in retry with exponential backoff
- **Cancellation**: Full CancellationToken support
- **Logging**: Microsoft.Extensions.Logging integration

### Requirements

- .NET 10.0 or later

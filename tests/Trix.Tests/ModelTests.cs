using System.Text.Json;
using FluentAssertions;
using Trix.Models;
using Trix.Resources;

namespace Trix.Tests;

public class ModelTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void Memory_Serialization_RoundTrip()
    {
        // Arrange
        var memory = new Memory
        {
            Id = "mem_123",
            Content = "Test content",
            Type = MemoryType.Text,
            Tags = new List<string> { "tag1", "tag2" },
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var json = JsonSerializer.Serialize(memory, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<Memory>(json, JsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(memory.Id);
        deserialized.Content.Should().Be(memory.Content);
        deserialized.Type.Should().Be(MemoryType.Text);
        deserialized.Tags.Should().BeEquivalentTo(memory.Tags);
    }

    [Fact]
    public void CreateMemoryRequest_Serialization()
    {
        // Arrange
        var request = new CreateMemoryRequest
        {
            Content = "Test content",
            Type = MemoryType.Markdown,
            Tags = new List<string> { "test" }
        };

        // Act
        var json = JsonSerializer.Serialize(request, JsonOptions);

        // Assert
        json.Should().Contain("\"content\":\"Test content\"");
        json.Should().Contain("\"type\":\"Markdown\"");
    }

    [Fact]
    public void Relationship_Serialization_RoundTrip()
    {
        // Arrange
        var relationship = new Relationship
        {
            Id = "rel_123",
            SourceId = "mem_1",
            TargetId = "mem_2",
            RelationshipType = RelationshipTypes.RelatedTo,
            Strength = 0.8,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var json = JsonSerializer.Serialize(relationship, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<Relationship>(json, JsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(relationship.Id);
        deserialized.SourceId.Should().Be(relationship.SourceId);
        deserialized.TargetId.Should().Be(relationship.TargetId);
        deserialized.Strength.Should().Be(0.8);
    }

    [Fact]
    public void Cluster_Serialization_RoundTrip()
    {
        // Arrange
        var cluster = new Cluster
        {
            Id = "cluster_123",
            Name = "Test Cluster",
            Description = "A test cluster",
            MemoryIds = new List<string> { "mem_1", "mem_2" },
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var json = JsonSerializer.Serialize(cluster, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<Cluster>(json, JsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(cluster.Id);
        deserialized.Name.Should().Be(cluster.Name);
        deserialized.MemoryIds.Should().BeEquivalentTo(cluster.MemoryIds);
    }

    [Fact]
    public void Space_Serialization_RoundTrip()
    {
        // Arrange
        var space = new Space
        {
            Id = "space_123",
            Slug = "test-space",
            Name = "Test Space",
            Description = "A test space",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var json = JsonSerializer.Serialize(space, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<Space>(json, JsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(space.Id);
        deserialized.Name.Should().Be(space.Name);
    }

    [Fact]
    public void Fact_Serialization_RoundTrip()
    {
        // Arrange
        var fact = new Fact
        {
            Id = "fact_123",
            Subject = "Albert Einstein",
            Predicate = "was_born_in",
            Object = "Ulm, Germany",
            Confidence = 0.95,
            SubjectType = FactNodeType.Entity,
            ObjectType = FactNodeType.Text,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var json = JsonSerializer.Serialize(fact, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<Fact>(json, JsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Subject.Should().Be(fact.Subject);
        deserialized.Predicate.Should().Be(fact.Predicate);
        deserialized.Object.Should().Be(fact.Object);
        deserialized.Confidence.Should().Be(0.95);
    }

    [Fact]
    public void Entity_Serialization_RoundTrip()
    {
        // Arrange
        var entity = new Entity
        {
            Id = "ent_123",
            Name = "Albert Einstein",
            Type = "person",
            Aliases = new List<string> { "Einstein", "A. Einstein" },
            Properties = new Dictionary<string, object>
            {
                { "birthYear", 1879 }
            },
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var json = JsonSerializer.Serialize(entity, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<Entity>(json, JsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Name.Should().Be(entity.Name);
        deserialized.Type.Should().Be("person");
        deserialized.Aliases.Should().BeEquivalentTo(entity.Aliases);
    }

    [Fact]
    public void PaginatedResponse_Serialization()
    {
        // Arrange
        var json = """
        {
            "data": [
                { "id": "mem_1", "content": "Content 1", "type": "text", "createdAt": "2025-01-01T00:00:00Z", "updatedAt": "2025-01-01T00:00:00Z" },
                { "id": "mem_2", "content": "Content 2", "type": "text", "createdAt": "2025-01-01T00:00:00Z", "updatedAt": "2025-01-01T00:00:00Z" }
            ],
            "pagination": {
                "total": 100,
                "page": 1,
                "limit": 10,
                "hasMore": true
            }
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<PaginatedResponse<Memory>>(json, JsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Data.Should().HaveCount(2);
        response.Pagination.Should().NotBeNull();
        response.Pagination!.Total.Should().Be(100);
        response.Pagination.HasMore.Should().BeTrue();
    }

    [Fact]
    public void MemoryType_EnumValues()
    {
        // Assert
        MemoryType.Text.Should().Be(MemoryType.Text);
        MemoryType.Markdown.Should().Be(MemoryType.Markdown);
        MemoryType.Url.Should().Be(MemoryType.Url);
        MemoryType.Audio.Should().Be(MemoryType.Audio);
    }

    [Fact]
    public void SearchMode_EnumValues()
    {
        // Assert
        SearchMode.Semantic.Should().Be(SearchMode.Semantic);
        SearchMode.Keyword.Should().Be(SearchMode.Keyword);
        SearchMode.Hybrid.Should().Be(SearchMode.Hybrid);
    }

    [Fact]
    public void Webhook_Serialization_RoundTrip()
    {
        // Arrange
        var webhook = new Webhook
        {
            Id = "wh_123",
            Url = "https://example.com/webhook",
            Events = new List<string> { "memory.created", "memory.updated" },
            Secret = "secret123",
            Active = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var json = JsonSerializer.Serialize(webhook, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<Webhook>(json, JsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(webhook.Id);
        deserialized.Url.Should().Be(webhook.Url);
        deserialized.Events.Should().BeEquivalentTo(webhook.Events);
        deserialized.Active.Should().BeTrue();
    }

    [Fact]
    public void Session_Serialization_RoundTrip()
    {
        // Arrange
        var session = new Session
        {
            Id = "sess_123",
            Name = "Test Session",
            Metadata = new Dictionary<string, object> { { "key", "value" } },
            StartedAt = DateTime.UtcNow
        };

        // Act
        var json = JsonSerializer.Serialize(session, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<Session>(json, JsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(session.Id);
        deserialized.Name.Should().Be(session.Name);
    }

    [Fact]
    public void Job_Serialization_RoundTrip()
    {
        // Arrange
        var job = new Job
        {
            Id = "job_123",
            Name = "enrichment",
            Queue = "default",
            Status = JobStatus.Completed,
            Progress = 100,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var json = JsonSerializer.Serialize(job, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<Job>(json, JsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(job.Id);
        deserialized.Status.Should().Be(JobStatus.Completed);
        deserialized.Progress.Should().Be(100);
    }

    [Fact]
    public void Highlight_Serialization_RoundTrip()
    {
        // Arrange
        var highlight = new Highlight
        {
            Id = "hl_123",
            MemoryId = "mem_123",
            Text = "Important text",
            StartOffset = 10,
            EndOffset = 25,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var json = JsonSerializer.Serialize(highlight, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<Highlight>(json, JsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(highlight.Id);
        deserialized.Text.Should().Be(highlight.Text);
        deserialized.StartOffset.Should().Be(10);
        deserialized.EndOffset.Should().Be(25);
    }

    [Fact]
    public void Enrichment_Serialization_RoundTrip()
    {
        // Arrange
        var enrichment = new Enrichment
        {
            Type = EnrichmentType.Summary,
            Status = EnrichmentStatus.Completed,
            Data = new Dictionary<string, object> { { "summary", "Test summary" } },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var json = JsonSerializer.Serialize(enrichment, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<Enrichment>(json, JsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Type.Should().Be(EnrichmentType.Summary);
        deserialized.Status.Should().Be(EnrichmentStatus.Completed);
    }

    [Fact]
    public void GraphTraversalResult_Serialization()
    {
        // Arrange
        var json = """
        {
            "nodes": [
                { "id": "node_1", "type": "memory", "label": "Node 1", "depth": 0 }
            ],
            "edges": [
                { "source": "node_1", "target": "node_2", "relationshipType": "related_to", "strength": 0.8 }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<GraphTraversalResult>(json, JsonOptions);

        // Assert
        result.Should().NotBeNull();
        result!.Nodes.Should().HaveCount(1);
        result.Edges.Should().HaveCount(1);
        result.Nodes[0].Id.Should().Be("node_1");
        result.Edges[0].Strength.Should().Be(0.8);
    }

    [Fact]
    public void SimilarMemory_Serialization()
    {
        // Arrange
        var json = """
        {
            "memory": { "id": "mem_1", "content": "Test", "createdAt": "2025-01-01T00:00:00Z", "updatedAt": "2025-01-01T00:00:00Z" },
            "similarity": 0.95
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<SimilarMemory>(json, JsonOptions);

        // Assert
        result.Should().NotBeNull();
        result!.Memory.Id.Should().Be("mem_1");
        result.Similarity.Should().Be(0.95);
    }

    [Fact]
    public void EnrichmentType_EnumValues()
    {
        // Assert
        EnrichmentType.Summary.Should().Be(EnrichmentType.Summary);
        EnrichmentType.Keywords.Should().Be(EnrichmentType.Keywords);
        EnrichmentType.Entities.Should().Be(EnrichmentType.Entities);
        EnrichmentType.Topics.Should().Be(EnrichmentType.Topics);
        EnrichmentType.Sentiment.Should().Be(EnrichmentType.Sentiment);
    }

    [Fact]
    public void JobStatus_EnumValues()
    {
        // Assert
        JobStatus.Waiting.Should().Be(JobStatus.Waiting);
        JobStatus.Active.Should().Be(JobStatus.Active);
        JobStatus.Completed.Should().Be(JobStatus.Completed);
        JobStatus.Failed.Should().Be(JobStatus.Failed);
    }

    [Fact]
    public void FeedbackType_EnumValues()
    {
        // Assert
        FeedbackType.Positive.Should().Be(FeedbackType.Positive);
        FeedbackType.Negative.Should().Be(FeedbackType.Negative);
        FeedbackType.Neutral.Should().Be(FeedbackType.Neutral);
    }

    [Fact]
    public void TranscriptResult_BasicDeserialization()
    {
        // Arrange - Basic transcript without advanced features
        var json = """
        {
            "memoryId": "mem_123",
            "text": "Hello world",
            "duration": 5.5,
            "language": "en",
            "provider": "assemblyai"
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<TranscriptResult>(json, JsonOptions);

        // Assert
        result.Should().NotBeNull();
        result!.MemoryId.Should().Be("mem_123");
        result.Text.Should().Be("Hello world");
        result.Duration.Should().Be(5.5);
        result.Language.Should().Be("en");
        result.Provider.Should().Be("assemblyai");
    }

    [Fact]
    public void TranscriptResult_FullDeserialization_WithAllFeatures()
    {
        // Arrange - Complete transcript with speaker diarization, entities, chapters, and content safety
        var json = """
        {
            "memoryId": "mem_123",
            "audioFileId": "file_456",
            "text": "Speaker A: Hello. Speaker B: Hi there.",
            "duration": 10.5,
            "language": "en",
            "languageConfidence": 0.98,
            "provider": "assemblyai",
            "summary": "A greeting conversation between two people.",
            "contentSafetyLabels": [
                {
                    "label": "profanity",
                    "confidence": 0.85,
                    "severity": "low",
                    "timestamp": { "start": 1.5, "end": 2.0 }
                }
            ],
            "providerMetadata": {
                "modelVersion": "v2.1",
                "processingTime": 3.2
            },
            "segments": [
                {
                    "id": "seg_1",
                    "startTime": 0.0,
                    "endTime": 2.5,
                    "text": "Hello",
                    "segmentIndex": 0,
                    "confidence": 0.95,
                    "speaker": "A",
                    "words": [
                        {
                            "word": "Hello",
                            "start": 0.0,
                            "end": 0.5,
                            "confidence": 0.95,
                            "speaker": "A"
                        }
                    ],
                    "wordConfidenceAvg": 0.95
                }
            ],
            "entities": [
                {
                    "id": "ent_1",
                    "entityType": "person",
                    "text": "John Smith",
                    "startTime": 1.0,
                    "endTime": 2.0,
                    "confidence": 0.92,
                    "metadata": {
                        "category": "name"
                    }
                }
            ],
            "chapters": [
                {
                    "id": "ch_1",
                    "chapterIndex": 0,
                    "headline": "Introduction",
                    "summary": "Initial greetings",
                    "gist": "Greetings",
                    "startTime": 0.0,
                    "endTime": 5.0
                }
            ],
            "words": [
                {
                    "word": "Hello",
                    "start": 0.0,
                    "end": 0.5,
                    "confidence": 0.95,
                    "speaker": "A"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<TranscriptResult>(json, JsonOptions);

        // Assert - Basic fields
        result.Should().NotBeNull();
        result!.MemoryId.Should().Be("mem_123");
        result.AudioFileId.Should().Be("file_456");
        result.Text.Should().Contain("Speaker A");
        result.Duration.Should().Be(10.5);
        result.Language.Should().Be("en");
        result.LanguageConfidence.Should().Be(0.98);
        result.Provider.Should().Be("assemblyai");
        result.Summary.Should().Be("A greeting conversation between two people.");

        // Assert - Content safety
        result.ContentSafetyLabels.Should().NotBeNull();
        result.ContentSafetyLabels.Should().HaveCount(1);
        result.ContentSafetyLabels![0].Label.Should().Be("profanity");
        result.ContentSafetyLabels[0].Confidence.Should().Be(0.85);
        result.ContentSafetyLabels[0].Severity.Should().Be("low");
        result.ContentSafetyLabels[0].Timestamp.Should().NotBeNull();
        result.ContentSafetyLabels[0].Timestamp!.Start.Should().Be(1.5);
        result.ContentSafetyLabels[0].Timestamp.End.Should().Be(2.0);

        // Assert - Provider metadata
        result.ProviderMetadata.Should().NotBeNull();
        result.ProviderMetadata.Should().ContainKey("modelVersion");

        // Assert - Segments with speaker diarization
        result.Segments.Should().NotBeNull();
        result.Segments.Should().HaveCount(1);
        result.Segments![0].Id.Should().Be("seg_1");
        result.Segments[0].Speaker.Should().Be("A");
        result.Segments[0].Text.Should().Be("Hello");
        result.Segments[0].Confidence.Should().Be(0.95);
        result.Segments[0].Words.Should().HaveCount(1);
        result.Segments[0].Words![0].Speaker.Should().Be("A");

        // Assert - Entities
        result.Entities.Should().NotBeNull();
        result.Entities.Should().HaveCount(1);
        result.Entities![0].Id.Should().Be("ent_1");
        result.Entities[0].EntityType.Should().Be("person");
        result.Entities[0].Text.Should().Be("John Smith");
        result.Entities[0].Confidence.Should().Be(0.92);

        // Assert - Chapters
        result.Chapters.Should().NotBeNull();
        result.Chapters.Should().HaveCount(1);
        result.Chapters![0].Id.Should().Be("ch_1");
        result.Chapters[0].Headline.Should().Be("Introduction");
        result.Chapters[0].Summary.Should().Be("Initial greetings");
        result.Chapters[0].Gist.Should().Be("Greetings");

        // Assert - Words
        result.Words.Should().NotBeNull();
        result.Words.Should().HaveCount(1);
        result.Words![0].Word.Should().Be("Hello");
        result.Words[0].Speaker.Should().Be("A");
    }

    [Fact]
    public void TranscriptSegment_Deserialization()
    {
        // Arrange
        var json = """
        {
            "id": "seg_1",
            "startTime": 0.0,
            "endTime": 5.5,
            "text": "This is a segment",
            "segmentIndex": 0,
            "confidence": 0.92,
            "speaker": "A",
            "wordConfidenceAvg": 0.91
        }
        """;

        // Act
        var segment = JsonSerializer.Deserialize<TranscriptSegment>(json, JsonOptions);

        // Assert
        segment.Should().NotBeNull();
        segment!.Id.Should().Be("seg_1");
        segment.StartTime.Should().Be(0.0);
        segment.EndTime.Should().Be(5.5);
        segment.Text.Should().Be("This is a segment");
        segment.SegmentIndex.Should().Be(0);
        segment.Confidence.Should().Be(0.92);
        segment.Speaker.Should().Be("A");
        segment.WordConfidenceAvg.Should().Be(0.91);
    }

    [Fact]
    public void TranscriptEntity_Deserialization()
    {
        // Arrange
        var json = """
        {
            "id": "ent_123",
            "entityType": "organization",
            "text": "Acme Corp",
            "startTime": 2.5,
            "endTime": 3.0,
            "confidence": 0.88,
            "metadata": {
                "industry": "technology"
            }
        }
        """;

        // Act
        var entity = JsonSerializer.Deserialize<TranscriptEntity>(json, JsonOptions);

        // Assert
        entity.Should().NotBeNull();
        entity!.Id.Should().Be("ent_123");
        entity.EntityType.Should().Be("organization");
        entity.Text.Should().Be("Acme Corp");
        entity.StartTime.Should().Be(2.5);
        entity.EndTime.Should().Be(3.0);
        entity.Confidence.Should().Be(0.88);
        entity.Metadata.Should().ContainKey("industry");
    }

    [Fact]
    public void TranscriptChapter_Deserialization()
    {
        // Arrange
        var json = """
        {
            "id": "ch_1",
            "chapterIndex": 0,
            "headline": "Opening Remarks",
            "summary": "The speaker introduces the main topic",
            "gist": "Introduction",
            "startTime": 0.0,
            "endTime": 60.0
        }
        """;

        // Act
        var chapter = JsonSerializer.Deserialize<TranscriptChapter>(json, JsonOptions);

        // Assert
        chapter.Should().NotBeNull();
        chapter!.Id.Should().Be("ch_1");
        chapter.ChapterIndex.Should().Be(0);
        chapter.Headline.Should().Be("Opening Remarks");
        chapter.Summary.Should().Be("The speaker introduces the main topic");
        chapter.Gist.Should().Be("Introduction");
        chapter.StartTime.Should().Be(0.0);
        chapter.EndTime.Should().Be(60.0);
    }

    [Fact]
    public void ContentSafetyLabel_Deserialization()
    {
        // Arrange
        var json = """
        {
            "label": "hate_speech",
            "confidence": 0.95,
            "severity": "high",
            "timestamp": {
                "start": 10.5,
                "end": 12.0
            }
        }
        """;

        // Act
        var label = JsonSerializer.Deserialize<ContentSafetyLabel>(json, JsonOptions);

        // Assert
        label.Should().NotBeNull();
        label!.Label.Should().Be("hate_speech");
        label.Confidence.Should().Be(0.95);
        label.Severity.Should().Be("high");
        label.Timestamp.Should().NotBeNull();
        label.Timestamp!.Start.Should().Be(10.5);
        label.Timestamp.End.Should().Be(12.0);
    }

    [Fact]
    public void WordTimestamp_WithSpeaker_Deserialization()
    {
        // Arrange
        var json = """
        {
            "word": "Hello",
            "start": 0.0,
            "end": 0.5,
            "confidence": 0.98,
            "speaker": "B"
        }
        """;

        // Act
        var word = JsonSerializer.Deserialize<WordTimestamp>(json, JsonOptions);

        // Assert
        word.Should().NotBeNull();
        word!.Word.Should().Be("Hello");
        word.Start.Should().Be(0.0);
        word.End.Should().Be(0.5);
        word.Confidence.Should().Be(0.98);
        word.Speaker.Should().Be("B");
    }
}

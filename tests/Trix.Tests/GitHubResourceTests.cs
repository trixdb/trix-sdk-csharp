using System.Net;
using System.Text;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Trix;
using Trix.Models;

namespace Trix.Tests;

/// <summary>
/// Tests for GitHub project analytics resource (ADR-152).
/// </summary>
public class GitHubResourceTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly TrixClient _client;

    public GitHubResourceTests()
    {
        _mockHandler = new Mock<HttpMessageHandler>();
        var options = new TrixClientOptions
        {
            ApiKey = "test_key",
            BaseUrl = "https://api.test.com",
            HttpHandler = _mockHandler.Object
        };
        _client = new TrixClient(options);
    }

    public void Dispose() => _client.Dispose();

    private void SetupResponse(HttpStatusCode statusCode, string content)
    {
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
    }

    private void SetupResponse(HttpStatusCode statusCode, string content,
        Action<HttpRequestMessage> requestInspector)
    {
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => requestInspector(req))
            .ReturnsAsync(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
    }

    [Fact]
    public async Task GitHub_IsExposedOnClient()
    {
        _client.GitHub.Should().NotBeNull();
    }

    [Fact]
    public async Task ListConnectionsAsync_ReturnsConnections()
    {
        const string json = """
        {
            "connections": [
                {
                    "id": "conn-1",
                    "project_id": "proj-1",
                    "repo_full_name": "owner/repo",
                    "webhook_active": true,
                    "sync_commits": true,
                    "sync_pull_requests": true,
                    "sync_issues": false,
                    "pr_review_bot_enabled": false
                }
            ]
        }
        """;

        HttpRequestMessage? captured = null;
        SetupResponse(HttpStatusCode.OK, json, req => captured = req);

        var result = await _client.GitHub.ListConnectionsAsync("proj-1");

        result.Connections.Should().HaveCount(1);
        result.Connections[0].Id.Should().Be("conn-1");
        result.Connections[0].RepoFullName.Should().Be("owner/repo");
        result.Connections[0].WebhookActive.Should().BeTrue();
        captured!.RequestUri!.PathAndQuery.Should().Contain("/v1/projects/proj-1/github");
    }

    [Fact]
    public async Task ListConnectionsAsync_EmptyProjectId_Throws()
    {
        await _client.GitHub.Invoking(g => g.ListConnectionsAsync(""))
            .Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetActivityAsync_ReturnsMemories()
    {
        const string json = """
        {
            "memories": [
                {
                    "id": "mem-1",
                    "type": "commit",
                    "content": "fix: resolve auth timeout issue",
                    "created_at": "2026-04-24T10:00:00Z",
                    "tags": ["source:github", "type:commit"]
                }
            ],
            "total": 1
        }
        """;

        HttpRequestMessage? captured = null;
        SetupResponse(HttpStatusCode.OK, json, req => captured = req);

        var result = await _client.GitHub.GetActivityAsync("proj-1", type: "commit", limit: 10);

        result.Memories.Should().HaveCount(1);
        result.Memories[0].Type.Should().Be("commit");
        result.Memories[0].Tags.Should().Contain("source:github");
        result.Total.Should().Be(1);
        captured!.RequestUri!.Query.Should().Contain("type=commit");
        captured.RequestUri.Query.Should().Contain("limit=10");
    }

    [Fact]
    public async Task GetVelocityAsync_ReturnsMetrics()
    {
        const string json = """
        {
            "merged_last_7_days": 5,
            "merged_last_30_days": 18,
            "avg_cycle_time_hours": 36.5,
            "avg_cycle_time_days": 1.5
        }
        """;

        SetupResponse(HttpStatusCode.OK, json);

        var result = await _client.GitHub.GetVelocityAsync("proj-1");

        result.MergedLast7Days.Should().Be(5);
        result.MergedLast30Days.Should().Be(18);
        result.AvgCycleTimeDays.Should().Be(1.5);
        result.AvgCycleTimeHours.Should().Be(36.5);
    }

    [Fact]
    public async Task GetVelocityAsync_NullCycleTime_ReturnsNull()
    {
        const string json = """
        {
            "merged_last_7_days": 0,
            "merged_last_30_days": 0,
            "avg_cycle_time_hours": null,
            "avg_cycle_time_days": null
        }
        """;

        SetupResponse(HttpStatusCode.OK, json);

        var result = await _client.GitHub.GetVelocityAsync("proj-1");

        result.AvgCycleTimeDays.Should().BeNull();
        result.AvgCycleTimeHours.Should().BeNull();
    }

    [Fact]
    public async Task GetFlaggedPRsAsync_ReturnsFlaggedList()
    {
        const string json = """
        {
            "prs": [
                {
                    "id": "mem-pr-1",
                    "summary": "fix: auth timeout — touches billing module unexpectedly",
                    "flags": ["pr:scope-creep", "pr:touches-hotspots"],
                    "created_at": "2026-04-20T09:00:00Z"
                }
            ],
            "total": 1
        }
        """;

        SetupResponse(HttpStatusCode.OK, json);

        var result = await _client.GitHub.GetFlaggedPRsAsync("proj-1");

        result.PRs.Should().HaveCount(1);
        result.PRs[0].Flags.Should().Contain("pr:scope-creep");
        result.PRs[0].Flags.Should().Contain("pr:touches-hotspots");
        result.Total.Should().Be(1);
    }

    [Fact]
    public async Task GetFlaggedPRsAsync_EmptyList_ReturnsEmpty()
    {
        const string json = """{ "prs": [], "total": 0 }""";
        SetupResponse(HttpStatusCode.OK, json);

        var result = await _client.GitHub.GetFlaggedPRsAsync("proj-1");

        result.PRs.Should().BeEmpty();
        result.Total.Should().Be(0);
    }

    [Fact]
    public async Task GetCycleTimeAsync_ReturnsTrends()
    {
        const string json = """
        {
            "avg_cycle_days_last_30": 4.2,
            "avg_cycle_days_30_60": 5.8,
            "avg_open_age_days": 12.3,
            "open_issue_count": 7,
            "closed_last_30": 14,
            "trend": "improving"
        }
        """;

        SetupResponse(HttpStatusCode.OK, json);

        var result = await _client.GitHub.GetCycleTimeAsync("proj-1");

        result.AvgCycleDaysLast30.Should().Be(4.2);
        result.AvgCycleDays3060.Should().Be(5.8);
        result.Trend.Should().Be("improving");
        result.OpenIssueCount.Should().Be(7);
        result.ClosedLast30.Should().Be(14);
    }

    [Fact]
    public async Task GetAgentAttributionAsync_ReturnsBreakdown()
    {
        const string json = """
        {
            "total_commits": 120,
            "total_prs": 45,
            "agent_breakdown": { "claude": 30, "copilot": 12 },
            "agent_total": 42,
            "human_total": 78,
            "agent_ratio": 0.35
        }
        """;

        SetupResponse(HttpStatusCode.OK, json);

        var result = await _client.GitHub.GetAgentAttributionAsync("proj-1");

        result.TotalCommits.Should().Be(120);
        result.TotalPRs.Should().Be(45);
        result.AgentBreakdown.Should().ContainKey("claude").WhoseValue.Should().Be(30);
        result.AgentBreakdown.Should().ContainKey("copilot").WhoseValue.Should().Be(12);
        result.AgentRatio.Should().Be(0.35);
    }

    [Fact]
    public async Task GenerateNarrativeAsync_PostsWithWindowDays()
    {
        const string json = """
        {
            "narrative": "• Shipped 5 PRs including auth fix and billing improvements\\n• Closed 3 issues",
            "stored": true,
            "window_days": 7
        }
        """;

        HttpRequestMessage? captured = null;
        SetupResponse(HttpStatusCode.OK, json, req => captured = req);

        var result = await _client.GitHub.GenerateNarrativeAsync("proj-1", windowDays: 7);

        result.Narrative.Should().StartWith("•");
        result.Stored.Should().BeTrue();
        result.WindowDays.Should().Be(7);
        captured!.Method.Should().Be(HttpMethod.Post);
        captured.RequestUri!.PathAndQuery.Should().Contain("/v1/projects/proj-1/github/narrative");
    }

    [Fact]
    public async Task UpdateConnectionAsync_PatchesSettings()
    {
        const string json = """
        {
            "id": "conn-1",
            "project_id": "proj-1",
            "repo_full_name": "owner/repo",
            "webhook_active": false,
            "sync_commits": true,
            "sync_pull_requests": true,
            "sync_issues": false,
            "pr_review_bot_enabled": true
        }
        """;

        HttpRequestMessage? captured = null;
        SetupResponse(HttpStatusCode.OK, json, req => captured = req);

        var request = new UpdateGitHubConnectionRequest
        {
            WebhookActive = false,
            PrReviewBotEnabled = true
        };

        var result = await _client.GitHub.UpdateConnectionAsync("proj-1", "conn-1", request);

        result.WebhookActive.Should().BeFalse();
        result.PrReviewBotEnabled.Should().BeTrue();
        captured!.Method.Should().Be(HttpMethod.Patch);
        captured.RequestUri!.PathAndQuery.Should().Contain("/v1/projects/proj-1/github/conn-1");
    }

    [Fact]
    public async Task DeleteConnectionAsync_SendsDelete()
    {
        HttpRequestMessage? captured = null;
        SetupResponse(HttpStatusCode.NoContent, "", req => captured = req);

        await _client.GitHub.DeleteConnectionAsync("proj-1", "conn-1");

        captured!.Method.Should().Be(HttpMethod.Delete);
        captured.RequestUri!.PathAndQuery.Should().Contain("/v1/projects/proj-1/github/conn-1");
    }

    [Fact]
    public async Task UpdateConnectionAsync_NullRequest_Throws()
    {
        await _client.GitHub.Invoking(g => g.UpdateConnectionAsync("proj-1", "conn-1", null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetActivityAsync_EncodesProjectIdInPath()
    {
        const string json = """{ "memories": [], "total": 0 }""";
        HttpRequestMessage? captured = null;
        SetupResponse(HttpStatusCode.OK, json, req => captured = req);

        await _client.GitHub.GetActivityAsync("proj-abc-123");

        captured!.RequestUri!.PathAndQuery.Should().Contain("proj-abc-123");
    }

    [Fact]
    public async Task ScanRepoAsync_PostsToScanEndpoint()
    {
        const string json = """
        {
            "scanned": true,
            "commits": 12,
            "prs": 4,
            "issues": 7,
            "files": 28,
            "hotspots": 3,
            "pr_briefs": 2
        }
        """;

        HttpRequestMessage? captured = null;
        SetupResponse(HttpStatusCode.OK, json, req => captured = req);

        var result = await _client.GitHub.ScanRepoAsync("proj-1", "conn-1");

        result.Scanned.Should().BeTrue();
        result.Commits.Should().Be(12);
        result.PRBriefs.Should().Be(2);
        captured!.Method.Should().Be(HttpMethod.Post);
        captured.RequestUri!.PathAndQuery.Should().Contain("/v1/projects/proj-1/github/conn-1/scan");
    }

    [Fact]
    public async Task ScanRepoAsync_EmptyProjectId_Throws()
    {
        await _client.GitHub.Invoking(g => g.ScanRepoAsync("", "conn-1"))
            .Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ScanRepoAsync_EmptyConnectionId_Throws()
    {
        await _client.GitHub.Invoking(g => g.ScanRepoAsync("proj-1", ""))
            .Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetGoalProgressAsync_ReturnsGoals()
    {
        const string json = """
        {
            "goals": [
                {
                    "id": "goal-1",
                    "title": "Reduce checkout error rate",
                    "progress": 0.65,
                    "status": "in_progress",
                    "progress_type": "percentage",
                    "last_github_progress": 0.60,
                    "last_github_updated_at": "2026-04-24T10:00:00Z"
                }
            ]
        }
        """;

        HttpRequestMessage? captured = null;
        SetupResponse(HttpStatusCode.OK, json, req => captured = req);

        var result = await _client.GitHub.GetGoalProgressAsync("proj-1");

        result.Goals.Should().HaveCount(1);
        result.Goals[0].Id.Should().Be("goal-1");
        result.Goals[0].Progress.Should().Be(0.65);
        result.Goals[0].LastGithubProgress.Should().Be(0.60);
        captured!.RequestUri!.PathAndQuery.Should().Contain("/v1/projects/proj-1/github/goal-progress");
    }

    [Fact]
    public async Task GetGoalProgressAsync_EmptyList_ReturnsEmpty()
    {
        const string json = """{ "goals": [] }""";
        SetupResponse(HttpStatusCode.OK, json);

        var result = await _client.GitHub.GetGoalProgressAsync("proj-1");

        result.Goals.Should().BeEmpty();
    }

    [Fact]
    public async Task GetReleaseReadinessAsync_ReturnsReport()
    {
        // Matches the current trix-api response shape (readinessScore / openIssues /
        // openPRs / recentMerges / topHotspots) — see github-release-readiness.js.
        const string json = """
        {
            "readinessScore": 72,
            "openIssues": {
                "count": 5,
                "blockerCount": 1,
                "blockers": [{ "number": 42, "title": "Fix auth" }]
            },
            "openPRs": {
                "count": 3,
                "unreviewedCount": 2,
                "staleCount": 1,
                "unreviewed": [],
                "stalePRs": []
            },
            "recentMerges": { "last7Days": 4 },
            "topHotspots": [{ "file": "src/app.ts", "changes": 12 }]
        }
        """;

        HttpRequestMessage? captured = null;
        SetupResponse(HttpStatusCode.OK, json, req => captured = req);

        var result = await _client.GitHub.GetReleaseReadinessAsync("proj-1");

        result.ReadinessScore.Should().Be(72);
        result.OpenIssues.Count.Should().Be(5);
        result.OpenIssues.BlockerCount.Should().Be(1);
        result.OpenPRs.Count.Should().Be(3);
        result.OpenPRs.UnreviewedCount.Should().Be(2);
        result.OpenPRs.StaleCount.Should().Be(1);
        result.TopHotspots.Should().HaveCount(1);
        captured!.RequestUri!.PathAndQuery.Should().Contain("/v1/projects/proj-1/github/release-readiness");
    }

    [Fact]
    public async Task GitHubAnalyticsMethods_HitCorrectPaths_RegressionForCs1503()
    {
        // Regression guard for issue #1: these 10 methods passed CancellationToken
        // positionally into the queryParams slot (CS1503) and had no tests. Exercising
        // each here means a re-break fails the test build, and asserts each hits its route.
        var cases = new (Func<Task> Call, string ExpectedPath)[]
        {
            (() => _client.GitHub.GetIssueThroughputAsync("proj-1", 8), "/v1/projects/proj-1/github/issue-throughput"),
            (() => _client.GitHub.GetIssueResolversAsync("proj-1", 30), "/v1/projects/proj-1/github/issue-resolvers"),
            (() => _client.GitHub.GetCycleTimeTrendAsync("proj-1", 8), "/v1/projects/proj-1/github/cycle-time-trend"),
            (() => _client.GitHub.GetPrMergeTimeAsync("proj-1", 90), "/v1/projects/proj-1/github/pr-merge-time"),
            (() => _client.GitHub.GetContributorMomentumAsync("proj-1", 28), "/v1/projects/proj-1/github/contributor-momentum"),
            (() => _client.GitHub.GetAgentAuditTrailAsync("proj-1", 90), "/v1/projects/proj-1/github/agent-audit"),
            (() => _client.GitHub.GetScopeCreepAsync("proj-1", 90), "/v1/projects/proj-1/github/scope-creep"),
            (() => _client.GitHub.GetAssigneeCycleTimeAsync("proj-1", 90), "/v1/projects/proj-1/github/assignee-cycle-time"),
            (() => _client.GitHub.GetPRTaskAlignmentAsync("proj-1", 90), "/v1/projects/proj-1/github/pr-task-alignment"),
            (() => _client.GitHub.GetTestGapAsync("proj-1", 90), "/v1/projects/proj-1/github/test-gap"),
        };

        foreach (var (call, expectedPath) in cases)
        {
            HttpRequestMessage? captured = null;
            SetupResponse(HttpStatusCode.OK, "{}", req => captured = req);
            await call();
            captured!.RequestUri!.PathAndQuery.Should().Contain(expectedPath);
        }
    }

    [Fact]
    public async Task GetLatestNarrativeAsync_ReturnsNarrative()
    {
        const string json = """
        {
            "narrative": {
                "id": "mem-narr-1",
                "content": "• Shipped 3 PRs\\n• Closed 5 issues",
                "created_at": "2026-04-24T09:00:00Z"
            }
        }
        """;

        HttpRequestMessage? captured = null;
        SetupResponse(HttpStatusCode.OK, json, req => captured = req);

        var result = await _client.GitHub.GetLatestNarrativeAsync("proj-1");

        result.Narrative.Should().NotBeNull();
        result.Narrative!.Id.Should().Be("mem-narr-1");
        result.Narrative.Content.Should().StartWith("•");
        captured!.RequestUri!.PathAndQuery.Should().Contain("/v1/projects/proj-1/github/narrative");
    }

    [Fact]
    public async Task GetLatestNarrativeAsync_NullNarrative_ReturnsNull()
    {
        const string json = """{ "narrative": null }""";
        SetupResponse(HttpStatusCode.OK, json);

        var result = await _client.GitHub.GetLatestNarrativeAsync("proj-1");

        result.Narrative.Should().BeNull();
    }

    [Fact]
    public async Task GetFileComplexityAsync_ReturnsMetrics()
    {
        const string json = """
        {
            "files": [{
                "file_path": "src/auth/service.ts",
                "repo_full_name": "acme/api",
                "language": "typescript",
                "cyclomatic_complexity": 18,
                "cognitive_complexity": 12,
                "loc": 320,
                "hotspot_score": 0.87,
                "complexity_level": "warning",
                "computed_at": "2026-04-20T10:00:00Z"
            }],
            "count": 1
        }
        """;
        HttpRequestMessage? captured = null;
        SetupResponse(HttpStatusCode.OK, json, req => captured = req);

        var result = await _client.GitHub.GetFileComplexityAsync("proj-1");

        result.Files.Should().HaveCount(1);
        result.Count.Should().Be(1);
        result.Files[0].FilePath.Should().Be("src/auth/service.ts");
        result.Files[0].ComplexityLevel.Should().Be("warning");
        result.Files[0].HotspotScore.Should().BeApproximately(0.87, 0.001);
        captured!.RequestUri!.PathAndQuery.Should().Contain("/github/complexity");
    }

    [Fact]
    public async Task GetFileComplexityAsync_WithFilters_AppendsQueryParams()
    {
        const string json = """{ "files": [], "count": 0 }""";
        HttpRequestMessage? captured = null;
        SetupResponse(HttpStatusCode.OK, json, req => captured = req);

        await _client.GitHub.GetFileComplexityAsync("proj-1", filePath: "src/auth.ts", repo: "acme/api");

        var query = captured!.RequestUri!.Query;
        query.Should().Contain("file=src%2Fauth.ts");
        query.Should().Contain("repo=acme%2Fapi");
    }

    [Fact]
    public async Task GetFileComplexityAsync_EmptyList_ReturnsEmpty()
    {
        const string json = """{ "files": [], "count": 0 }""";
        SetupResponse(HttpStatusCode.OK, json);

        var result = await _client.GitHub.GetFileComplexityAsync("proj-1");

        result.Files.Should().BeEmpty();
        result.Count.Should().Be(0);
    }
}

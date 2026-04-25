using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// A GitHub project connection linking a repository to a Trix project (ADR-152).
/// </summary>
public class GitHubProjectConnection
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("project_id")]
    public string ProjectId { get; set; } = string.Empty;

    [JsonPropertyName("repo_full_name")]
    public string RepoFullName { get; set; } = string.Empty;

    [JsonPropertyName("webhook_active")]
    public bool WebhookActive { get; set; }

    [JsonPropertyName("sync_commits")]
    public bool SyncCommits { get; set; }

    [JsonPropertyName("sync_pull_requests")]
    public bool SyncPullRequests { get; set; }

    [JsonPropertyName("sync_issues")]
    public bool SyncIssues { get; set; }

    [JsonPropertyName("pr_review_bot_enabled")]
    public bool PrReviewBotEnabled { get; set; }

    [JsonPropertyName("last_webhook_at")]
    public string? LastWebhookAt { get; set; }
}

/// <summary>
/// Response for listing GitHub project connections.
/// </summary>
public class GitHubConnectionsResponse
{
    [JsonPropertyName("connections")]
    public List<GitHubProjectConnection> Connections { get; set; } = new();
}

/// <summary>
/// A GitHub activity memory (commit, PR, issue).
/// </summary>
public class GitHubActivityMemory
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// Response for listing GitHub activity memories.
/// </summary>
public class GitHubActivityResponse
{
    [JsonPropertyName("memories")]
    public List<GitHubActivityMemory> Memories { get; set; } = new();

    [JsonPropertyName("total")]
    public int Total { get; set; }
}

/// <summary>
/// PR velocity metrics for a project.
/// </summary>
public class PRVelocityResponse
{
    [JsonPropertyName("merged_last_7_days")]
    public int MergedLast7Days { get; set; }

    [JsonPropertyName("merged_last_30_days")]
    public int MergedLast30Days { get; set; }

    [JsonPropertyName("avg_cycle_time_hours")]
    public double? AvgCycleTimeHours { get; set; }

    [JsonPropertyName("avg_cycle_time_days")]
    public double? AvgCycleTimeDays { get; set; }
}

/// <summary>
/// A risk-flagged pull request.
/// </summary>
public class FlaggedPR
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("flags")]
    public List<string> Flags { get; set; } = new();

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;
}

/// <summary>
/// Response for flagged pull requests.
/// </summary>
public class FlaggedPRsResponse
{
    [JsonPropertyName("prs")]
    public List<FlaggedPR> PRs { get; set; } = new();

    [JsonPropertyName("total")]
    public int Total { get; set; }
}

/// <summary>
/// Pre-review brief for a pull request, including quality score and risk signals.
/// </summary>
public class PRBrief
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("prNumber")]
    public int? PrNumber { get; set; }

    [JsonPropertyName("prUrl")]
    public string? PrUrl { get; set; }

    [JsonPropertyName("repo")]
    public string? Repo { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("briefContent")]
    public string BriefContent { get; set; } = string.Empty;

    [JsonPropertyName("riskFlags")]
    public List<string> RiskFlags { get; set; } = new();

    [JsonPropertyName("qualityScore")]
    public double? QualityScore { get; set; }

    /// <summary>AI assistant that authored this PR ('claude', 'copilot', 'cursor', 'gemini'), or null.</summary>
    [JsonPropertyName("agent")]
    public string? Agent { get; set; }

    [JsonPropertyName("isOpen")]
    public bool IsOpen { get; set; }

    [JsonPropertyName("hasTests")]
    public bool HasTests { get; set; }

    [JsonPropertyName("touchesHotspots")]
    public bool TouchesHotspots { get; set; }

    [JsonPropertyName("touchesLoadBearing")]
    public bool TouchesLoadBearing { get; set; }

    [JsonPropertyName("touchesClones")]
    public bool TouchesClones { get; set; }

    [JsonPropertyName("scopeCreep")]
    public bool ScopeCreep { get; set; }

    [JsonPropertyName("semanticDrift")]
    public bool SemanticDrift { get; set; }

    [JsonPropertyName("createdAt")]
    public string CreatedAt { get; set; } = string.Empty;
}

/// <summary>
/// Response for PR briefs list.
/// </summary>
public class PRBriefsResponse
{
    [JsonPropertyName("briefs")]
    public List<PRBrief> Briefs { get; set; } = new();

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; } = "open";
}

/// <summary>
/// Issue cycle time analytics for a project.
/// </summary>
public class CycleTimeResponse
{
    [JsonPropertyName("avg_cycle_days_last_30")]
    public double? AvgCycleDaysLast30 { get; set; }

    [JsonPropertyName("avg_cycle_days_30_60")]
    public double? AvgCycleDays3060 { get; set; }

    [JsonPropertyName("avg_open_age_days")]
    public double? AvgOpenAgeDays { get; set; }

    [JsonPropertyName("open_issue_count")]
    public int OpenIssueCount { get; set; }

    [JsonPropertyName("closed_last_30")]
    public int ClosedLast30 { get; set; }

    [JsonPropertyName("trend")]
    public string Trend { get; set; } = string.Empty;
}

/// <summary>
/// AI vs human commit/PR attribution for a project.
/// </summary>
public class AgentAttributionResponse
{
    [JsonPropertyName("total_commits")]
    public int TotalCommits { get; set; }

    [JsonPropertyName("total_prs")]
    public int TotalPRs { get; set; }

    [JsonPropertyName("agent_breakdown")]
    public Dictionary<string, int> AgentBreakdown { get; set; } = new();

    [JsonPropertyName("agent_total")]
    public int AgentTotal { get; set; }

    [JsonPropertyName("human_total")]
    public int HumanTotal { get; set; }

    [JsonPropertyName("agent_ratio")]
    public double AgentRatio { get; set; }

    /// <summary>Average PR quality score (0–100) per AI tool, keyed by agent name.</summary>
    [JsonPropertyName("agent_quality_scores")]
    public Dictionary<string, double?> AgentQualityScores { get; set; } = new();

    /// <summary>Average PR quality score (0–100) for human-authored PRs, or null if no data.</summary>
    [JsonPropertyName("human_avg_quality")]
    public double? HumanAvgQuality { get; set; }
}

/// <summary>
/// Generated delivery narrative.
/// </summary>
public class GenerateNarrativeResponse
{
    [JsonPropertyName("narrative")]
    public string Narrative { get; set; } = string.Empty;

    [JsonPropertyName("stored")]
    public bool Stored { get; set; }

    [JsonPropertyName("window_days")]
    public int WindowDays { get; set; }
}

/// <summary>
/// Response for POST /:connectionId/scan — backfill a linked repository.
/// </summary>
public class ScanRepoResponse
{
    [JsonPropertyName("scanned")]
    public bool Scanned { get; set; }

    [JsonPropertyName("commits")]
    public int Commits { get; set; }

    [JsonPropertyName("prs")]
    public int PRs { get; set; }

    [JsonPropertyName("issues")]
    public int Issues { get; set; }

    [JsonPropertyName("files")]
    public int Files { get; set; }

    [JsonPropertyName("hotspots")]
    public int Hotspots { get; set; }

    [JsonPropertyName("pr_briefs")]
    public int PRBriefs { get; set; }

    [JsonPropertyName("errors")]
    public List<string>? Errors { get; set; }
}

/// <summary>
/// A goal tracked via GitHub issue completion.
/// </summary>
public class LinkedGoal
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("title")] public string Title { get; set; } = string.Empty;
    [JsonPropertyName("progress")] public double Progress { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("progress_type")] public string? ProgressType { get; set; }
    [JsonPropertyName("last_github_progress")] public double? LastGithubProgress { get; set; }
    [JsonPropertyName("last_github_updated_at")] public string? LastGithubUpdatedAt { get; set; }
}

/// <summary>
/// Response for goal progress driven by GitHub issues.
/// </summary>
public class GoalProgressResponse
{
    [JsonPropertyName("goals")]
    public List<LinkedGoal> Goals { get; set; } = new();
}

/// <summary>
/// A single GitHub-driven goal progress event (PR merge → issue close → goal bump).
/// </summary>
public class GoalProgressEvent
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("goal_id")] public string GoalId { get; set; } = string.Empty;
    [JsonPropertyName("goal_title")] public string GoalTitle { get; set; } = string.Empty;
    [JsonPropertyName("goal_status")] public string GoalStatus { get; set; } = string.Empty;
    [JsonPropertyName("previous_progress")] public double PreviousProgress { get; set; }
    [JsonPropertyName("new_progress")] public double NewProgress { get; set; }
    [JsonPropertyName("note")] public string? Note { get; set; }
    [JsonPropertyName("created_at")] public string CreatedAt { get; set; } = string.Empty;
}

/// <summary>
/// Response for the goal progress history feed.
/// </summary>
public class GoalProgressHistoryResponse
{
    [JsonPropertyName("history")]
    public List<GoalProgressEvent> History { get; set; } = new();
}

/// <summary>
/// Per-signal breakdown for release readiness.
/// </summary>
public class ReleaseReadinessSignals
{
    [JsonPropertyName("open_prs")] public int OpenPRs { get; set; }
    [JsonPropertyName("blocking_tasks")] public int BlockingTasks { get; set; }
    [JsonPropertyName("goal_completion_pct")] public int GoalCompletionPct { get; set; }
    [JsonPropertyName("scope_creep_prs")] public int ScopeCreepPRs { get; set; }
}

/// <summary>
/// Release readiness score and detail lists.
/// </summary>
public class ReleaseReadinessResponse
{
    [JsonPropertyName("score")] public int Score { get; set; }
    [JsonPropertyName("ready")] public bool Ready { get; set; }
    [JsonPropertyName("signals")] public ReleaseReadinessSignals Signals { get; set; } = new();
}

/// <summary>
/// A stored narrative memory record.
/// </summary>
public class StoredNarrative
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("content")] public string Content { get; set; } = string.Empty;
    [JsonPropertyName("created_at")] public string CreatedAt { get; set; } = string.Empty;
}

/// <summary>
/// Response for fetching the latest stored narrative.
/// </summary>
public class LatestNarrativeResponse
{
    [JsonPropertyName("narrative")]
    public StoredNarrative? Narrative { get; set; }
}

/// <summary>
/// Cyclomatic and cognitive complexity metrics for a single tracked file.
/// </summary>
public class FileComplexityMetric
{
    [JsonPropertyName("file_path")] public string FilePath { get; set; } = string.Empty;
    [JsonPropertyName("repo_full_name")] public string RepoFullName { get; set; } = string.Empty;
    [JsonPropertyName("language")] public string? Language { get; set; }
    [JsonPropertyName("cyclomatic_complexity")] public int? CyclomaticComplexity { get; set; }
    [JsonPropertyName("cognitive_complexity")] public int? CognitiveComplexity { get; set; }
    [JsonPropertyName("loc")] public int? Loc { get; set; }
    [JsonPropertyName("hotspot_score")] public double? HotspotScore { get; set; }
    /// <summary>"ok" | "warning" | "critical"</summary>
    [JsonPropertyName("complexity_level")] public string? ComplexityLevel { get; set; }
    [JsonPropertyName("computed_at")] public string? ComputedAt { get; set; }
}

/// <summary>
/// Response for GET /github/complexity — file complexity metrics sorted by hotspot score.
/// </summary>
public class FileComplexityResponse
{
    [JsonPropertyName("files")]
    public List<FileComplexityMetric> Files { get; set; } = new();

    [JsonPropertyName("count")]
    public int Count { get; set; }
}

// ── Phase 5: Code Quality Scanner + Repo Stats ────────────────────────────

/// <summary>
/// A single code improvement suggestion (Dependabot / code scanning / LLM).
/// </summary>
public class CodeImprovement
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    /// <summary>"dependency"|"security"|"performance"|"refactor"|"maintenance"</summary>
    [JsonPropertyName("category")] public string Category { get; set; } = string.Empty;
    /// <summary>"critical"|"high"|"medium"|"low"</summary>
    [JsonPropertyName("priority")] public string Priority { get; set; } = string.Empty;
    [JsonPropertyName("title")] public string Title { get; set; } = string.Empty;
    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
    [JsonPropertyName("file_path")] public string? FilePath { get; set; }
    /// <summary>"open"|"dismissed"|"in_progress"|"resolved"</summary>
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    /// <summary>"rule"|"llm"</summary>
    [JsonPropertyName("generated_by")] public string GeneratedBy { get; set; } = string.Empty;
    [JsonPropertyName("generated_at")] public string GeneratedAt { get; set; } = string.Empty;
}

public class CodeImprovementsResponse
{
    [JsonPropertyName("suggestions")] public List<CodeImprovement> Suggestions { get; set; } = new();
}

public class ImprovementGenerateResponse
{
    [JsonPropertyName("generated")] public int Generated { get; set; }
    [JsonPropertyName("repo")] public string Repo { get; set; } = string.Empty;
}

public class ImprovementSummaryRow
{
    [JsonPropertyName("category")] public string Category { get; set; } = string.Empty;
    [JsonPropertyName("priority")] public string Priority { get; set; } = string.Empty;
    [JsonPropertyName("cnt")] public string Cnt { get; set; } = string.Empty;
}

public class ImprovementsHistoryItem
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("snapshotted_at")] public string SnapshottedAt { get; set; } = string.Empty;
    [JsonPropertyName("suggestion_count")] public int SuggestionCount { get; set; }
    [JsonPropertyName("critical_count")] public int CriticalCount { get; set; }
    [JsonPropertyName("warning_count")] public int WarningCount { get; set; }
    [JsonPropertyName("total_files")] public int TotalFiles { get; set; }
    [JsonPropertyName("hotspot_count")] public int HotspotCount { get; set; }
}

public class RepoLanguage
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("bytes")] public long Bytes { get; set; }
    [JsonPropertyName("pct")] public double Pct { get; set; }
}

public class RepoContributor
{
    [JsonPropertyName("login")] public string Login { get; set; } = string.Empty;
    [JsonPropertyName("avatar_url")] public string AvatarUrl { get; set; } = string.Empty;
    [JsonPropertyName("profile_url")] public string ProfileUrl { get; set; } = string.Empty;
    [JsonPropertyName("contributions")] public int Contributions { get; set; }
}

public class RepoMeta
{
    [JsonPropertyName("full_name")] public string FullName { get; set; } = string.Empty;
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("stars")] public int Stars { get; set; }
    [JsonPropertyName("forks")] public int Forks { get; set; }
    [JsonPropertyName("open_issues")] public int OpenIssues { get; set; }
    [JsonPropertyName("primary_language")] public string? PrimaryLanguage { get; set; }
    [JsonPropertyName("license")] public string? License { get; set; }
    [JsonPropertyName("is_private")] public bool IsPrivate { get; set; }
    [JsonPropertyName("size_kb")] public int SizeKb { get; set; }
}

public class LocalMetrics
{
    [JsonPropertyName("analyzed_files")] public int AnalyzedFiles { get; set; }
    [JsonPropertyName("total_loc")] public int TotalLoc { get; set; }
    [JsonPropertyName("hotspot_count")] public int HotspotCount { get; set; }
    [JsonPropertyName("critical_files")] public int CriticalFiles { get; set; }
    [JsonPropertyName("warning_files")] public int WarningFiles { get; set; }
}

public class RepoStatsData
{
    [JsonPropertyName("repo")] public RepoMeta? Repo { get; set; }
    [JsonPropertyName("languages")] public List<RepoLanguage> Languages { get; set; } = new();
    [JsonPropertyName("contributors")] public List<RepoContributor> Contributors { get; set; } = new();
    [JsonPropertyName("readme")] public string? Readme { get; set; }
    [JsonPropertyName("open_pr_count")] public int? OpenPrCount { get; set; }
    [JsonPropertyName("local_metrics")] public LocalMetrics LocalMetrics { get; set; } = new();
}

public class RepoStatsResponse
{
    [JsonPropertyName("stats")] public RepoStatsData Stats { get; set; } = new();
}

// Internal wrapper responses for array-valued endpoints
internal class ImprovementStatusWrapper
{
    [JsonPropertyName("suggestion")] public CodeImprovement Suggestion { get; set; } = new();
}

internal class ImprovementSummaryWrapper
{
    [JsonPropertyName("summary")] public List<ImprovementSummaryRow> Summary { get; set; } = new();
}

internal class ImprovementsHistoryWrapper
{
    [JsonPropertyName("history")] public List<ImprovementsHistoryItem> History { get; set; } = new();
}

/// <summary>
/// Request to update a GitHub project connection.
/// </summary>
public class UpdateGitHubConnectionRequest
{
    [JsonPropertyName("webhook_active")]
    public bool? WebhookActive { get; set; }

    [JsonPropertyName("sync_commits")]
    public bool? SyncCommits { get; set; }

    [JsonPropertyName("sync_pull_requests")]
    public bool? SyncPullRequests { get; set; }

    [JsonPropertyName("sync_issues")]
    public bool? SyncIssues { get; set; }

    [JsonPropertyName("pr_review_bot_enabled")]
    public bool? PrReviewBotEnabled { get; set; }
}

/// <summary>
/// Response when creating a GitHub issue from a code improvement suggestion.
/// </summary>
public class CreateIssueFromSuggestionResponse
{
    [JsonPropertyName("issue")]
    public CreatedGitHubIssue Issue { get; set; } = new();
}

/// <summary>
/// Metadata for a newly created GitHub issue.
/// </summary>
public class CreatedGitHubIssue
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}


/// <summary>One-call project health snapshot for agents.</summary>
public class HealthSnapshotRisk
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
}

public class HealthSnapshotQualityGate
{
    [JsonPropertyName("passed")]
    public bool? Passed { get; set; }

    [JsonPropertyName("avgMaintainabilityIndex")]
    public int? AvgMaintainabilityIndex { get; set; }

    [JsonPropertyName("totalFiles")]
    public int TotalFiles { get; set; }

    [JsonPropertyName("criticalFiles")]
    public int CriticalFiles { get; set; }
}

public class HealthSnapshotSuggestions
{
    [JsonPropertyName("critical")]
    public int Critical { get; set; }

    [JsonPropertyName("high")]
    public int High { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }
}

public class HealthSnapshotVelocity
{
    [JsonPropertyName("mergedLast7Days")]
    public int MergedLast7Days { get; set; }

    [JsonPropertyName("mergedLast30Days")]
    public int MergedLast30Days { get; set; }
}

public class HealthSnapshotOpenPRs
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("risky")]
    public int Risky { get; set; }

    [JsonPropertyName("avgQualityScore")]
    public double? AvgQualityScore { get; set; }
}

public class HealthSnapshotResponse
{
    [JsonPropertyName("qualityGate")]
    public HealthSnapshotQualityGate QualityGate { get; set; } = new();

    [JsonPropertyName("suggestions")]
    public HealthSnapshotSuggestions Suggestions { get; set; } = new();

    [JsonPropertyName("velocity")]
    public HealthSnapshotVelocity Velocity { get; set; } = new();

    [JsonPropertyName("openPRs")]
    public HealthSnapshotOpenPRs OpenPRs { get; set; } = new();

    [JsonPropertyName("topRisks")]
    public List<HealthSnapshotRisk> TopRisks { get; set; } = new();
}

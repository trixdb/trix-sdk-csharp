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

    [JsonPropertyName("prQualityTrend")]
    public HealthSnapshotPRQualityTrend? PrQualityTrend { get; set; }

    [JsonPropertyName("reviewTurnaround")]
    public HealthSnapshotReviewTurnaround? ReviewTurnaround { get; set; }

    [JsonPropertyName("urgentItems")]
    public HealthSnapshotUrgentItems? UrgentItems { get; set; }

    [JsonPropertyName("issueBacklog")]
    public HealthSnapshotIssueBacklog? IssueBacklog { get; set; }

    [JsonPropertyName("reviewCoverage")]
    public HealthSnapshotReviewCoverage? ReviewCoverage { get; set; }

    [JsonPropertyName("issueFlow")]
    public HealthSnapshotIssueFlow? IssueFlow { get; set; }
}

public class HealthSnapshotIssueFlow
{
    [JsonPropertyName("openedLast7d")]
    public int OpenedLast7d { get; set; }

    [JsonPropertyName("closedLast7d")]
    public int ClosedLast7d { get; set; }

    [JsonPropertyName("netFlow7d")]
    public int NetFlow7d { get; set; }
}

public class HealthSnapshotIssueBacklog
{
    [JsonPropertyName("totalOpen")]
    public int TotalOpen { get; set; }

    [JsonPropertyName("unassignedCount")]
    public int UnassignedCount { get; set; }
}

public class HealthSnapshotReviewCoverage
{
    [JsonPropertyName("totalMerged")]
    public int TotalMerged { get; set; }

    [JsonPropertyName("coveragePct")]
    public int? CoveragePct { get; set; }

    [JsonPropertyName("lookbackDays")]
    public int LookbackDays { get; set; } = 30;
}

public class HealthSnapshotPRQualityTrend
{
    [JsonPropertyName("direction")]
    public string Direction { get; set; } = "stable";

    [JsonPropertyName("currentAvg")]
    public double? CurrentAvg { get; set; }

    [JsonPropertyName("weekDelta")]
    public double? WeekDelta { get; set; }
}

public class HealthSnapshotReviewTurnaround
{
    [JsonPropertyName("avgHours")]
    public int? AvgHours { get; set; }

    [JsonPropertyName("unreviewedCount")]
    public int UnreviewedCount { get; set; }
}

public class HealthSnapshotUrgentItems
{
    [JsonPropertyName("critical")]
    public int Critical { get; set; }

    [JsonPropertyName("urgentTotal")]
    public int UrgentTotal { get; set; }
}

public class BranchInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("repoFullName")]
    public string? RepoFullName { get; set; }

    [JsonPropertyName("lastCommitAt")]
    public string? LastCommitAt { get; set; }

    [JsonPropertyName("commitCount")]
    public int CommitCount { get; set; }

    [JsonPropertyName("openPrNumber")]
    public int? OpenPrNumber { get; set; }

    [JsonPropertyName("openPrUrl")]
    public string? OpenPrUrl { get; set; }

    [JsonPropertyName("isDefault")]
    public bool IsDefault { get; set; }

    [JsonPropertyName("isStale")]
    public bool IsStale { get; set; }
}

public class ActiveBranchesResult
{
    [JsonPropertyName("branches")]
    public List<BranchInfo> Branches { get; set; } = new();

    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class ContributorQualityStat
{
    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    [JsonPropertyName("pr_count")]
    public int PrCount { get; set; }

    [JsonPropertyName("avg_quality")]
    public double? AvgQuality { get; set; }

    [JsonPropertyName("with_tests_count")]
    public int WithTestsCount { get; set; }

    [JsonPropertyName("test_coverage_pct")]
    public int TestCoveragePct { get; set; }

    [JsonPropertyName("avg_merge_days")]
    public double? AvgMergeDays { get; set; }

    [JsonPropertyName("last_active_at")]
    public string? LastActiveAt { get; set; }

    [JsonPropertyName("reviewsGiven")]
    public int ReviewsGiven { get; set; }

    [JsonPropertyName("approvals")]
    public int Approvals { get; set; }
}

public class ContributorQualityResult
{
    [JsonPropertyName("contributors")]
    public List<ContributorQualityStat> Contributors { get; set; } = new();

    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class OpenPRAging
{
    [JsonPropertyName("prNumber")]
    public int? PrNumber { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("repo")]
    public string? Repo { get; set; }

    [JsonPropertyName("headBranch")]
    public string? HeadBranch { get; set; }

    [JsonPropertyName("openedAt")]
    public string? OpenedAt { get; set; }

    [JsonPropertyName("lastUpdatedAt")]
    public string? LastUpdatedAt { get; set; }

    [JsonPropertyName("ageDays")]
    public int AgeDays { get; set; }

    [JsonPropertyName("isStale")]
    public bool IsStale { get; set; }

    [JsonPropertyName("requestedReviewers")]
    public List<string> RequestedReviewers { get; set; } = new();

    [JsonPropertyName("hasReview")]
    public bool HasReview { get; set; }
}

public class PrAgingResult
{
    [JsonPropertyName("prs")]
    public List<OpenPRAging> Prs { get; set; } = new();

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("staleDays")]
    public int StaleDays { get; set; }
}

public class PRSizeBucket
{
    [JsonPropertyName("size")]
    public string Size { get; set; } = string.Empty;

    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("pct")]
    public int Pct { get; set; }

    [JsonPropertyName("avgQuality")]
    public double? AvgQuality { get; set; }

    [JsonPropertyName("testCoveragePct")]
    public int TestCoveragePct { get; set; }
}

public class PrSizeDistributionResult
{
    [JsonPropertyName("distribution")]
    public List<PRSizeBucket> Distribution { get; set; } = new();

    [JsonPropertyName("total")]
    public int Total { get; set; }
}

public class ReviewAuthorStat
{
    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    [JsonPropertyName("reviewedCount")]
    public int ReviewedCount { get; set; }

    [JsonPropertyName("avgHours")]
    public double? AvgHours { get; set; }

    [JsonPropertyName("within24hCount")]
    public int Within24hCount { get; set; }

    [JsonPropertyName("within24hPct")]
    public int Within24hPct { get; set; }
}

public class ReviewTurnaroundResult
{
    [JsonPropertyName("avgHours")]
    public double? AvgHours { get; set; }

    [JsonPropertyName("reviewedWithin24hPct")]
    public int ReviewedWithin24hPct { get; set; }

    [JsonPropertyName("totalReviewed")]
    public int TotalReviewed { get; set; }

    [JsonPropertyName("unreviewedCount")]
    public int UnreviewedCount { get; set; }

    [JsonPropertyName("authorStats")]
    public List<ReviewAuthorStat> AuthorStats { get; set; } = new();
}

public class WorkQueueItem
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("priority")]
    public string Priority { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("detail")]
    public string Detail { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public class WorkQueueResult
{
    [JsonPropertyName("items")]
    public List<WorkQueueItem> Items { get; set; } = new();

    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class ReviewerPendingPR
{
    [JsonPropertyName("prNumber")]
    public int? PrNumber { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public class ReviewerWorkloadStat
{
    [JsonPropertyName("reviewer")]
    public string Reviewer { get; set; } = string.Empty;

    [JsonPropertyName("pendingCount")]
    public int PendingCount { get; set; }

    [JsonPropertyName("avgPendingAgeHours")]
    public int? AvgPendingAgeHours { get; set; }

    [JsonPropertyName("pendingPrs")]
    public List<ReviewerPendingPR> PendingPrs { get; set; } = new();

    [JsonPropertyName("totalReviews")]
    public int TotalReviews { get; set; }

    [JsonPropertyName("approvals")]
    public int Approvals { get; set; }

    [JsonPropertyName("avgResponseHours")]
    public double? AvgResponseHours { get; set; }
}

public class ReviewerWorkloadResult
{
    [JsonPropertyName("reviewers")]
    public List<ReviewerWorkloadStat> Reviewers { get; set; } = new();

    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class ApprovedPR
{
    [JsonPropertyName("prNumber")]
    public int? PrNumber { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("repo")]
    public string? Repo { get; set; }

    [JsonPropertyName("ageDays")]
    public int AgeDays { get; set; }

    [JsonPropertyName("approvalCount")]
    public int ApprovalCount { get; set; }

    [JsonPropertyName("approvers")]
    public List<string> Approvers { get; set; } = new();

    [JsonPropertyName("hasChangesRequested")]
    public bool HasChangesRequested { get; set; }
}

public class ApprovedPRsResult
{
    [JsonPropertyName("prs")]
    public List<ApprovedPR> Prs { get; set; } = new();

    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class IssueLabelCount
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = "";

    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class BacklogIssue
{
    [JsonPropertyName("issueNumber")]
    public int? IssueNumber { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("assignees")]
    public List<string> Assignees { get; set; } = new();

    [JsonPropertyName("labels")]
    public List<string> Labels { get; set; } = new();

    [JsonPropertyName("milestone")]
    public string? Milestone { get; set; }

    [JsonPropertyName("ageDays")]
    public int AgeDays { get; set; }
}

public class IssueBacklogResult
{
    [JsonPropertyName("totalOpen")]
    public int TotalOpen { get; set; }

    [JsonPropertyName("unassignedCount")]
    public int UnassignedCount { get; set; }

    [JsonPropertyName("unlabeledCount")]
    public int UnlabeledCount { get; set; }

    [JsonPropertyName("oldestAgeDays")]
    public int OldestAgeDays { get; set; }

    [JsonPropertyName("avgAgeDays")]
    public int AvgAgeDays { get; set; }

    [JsonPropertyName("labelDistribution")]
    public List<IssueLabelCount> LabelDistribution { get; set; } = new();

    [JsonPropertyName("oldestIssues")]
    public List<BacklogIssue> OldestIssues { get; set; } = new();
}

public class AuthorReviewCoverage
{
    [JsonPropertyName("author")]
    public string Author { get; set; } = "";

    [JsonPropertyName("mergedCount")]
    public int MergedCount { get; set; }

    [JsonPropertyName("reviewedCount")]
    public int ReviewedCount { get; set; }

    [JsonPropertyName("unreviewedCount")]
    public int UnreviewedCount { get; set; }

    [JsonPropertyName("coveragePct")]
    public int CoveragePct { get; set; }
}

public class ReviewCoverageResult
{
    [JsonPropertyName("totalMerged")]
    public int TotalMerged { get; set; }

    [JsonPropertyName("reviewedCount")]
    public int ReviewedCount { get; set; }

    [JsonPropertyName("unreviewedCount")]
    public int UnreviewedCount { get; set; }

    [JsonPropertyName("coveragePct")]
    public int? CoveragePct { get; set; }

    [JsonPropertyName("lookbackDays")]
    public int LookbackDays { get; set; } = 90;

    [JsonPropertyName("byAuthor")]
    public List<AuthorReviewCoverage> ByAuthor { get; set; } = new();
}

// ── Label Velocity ───────────────────────────────────────────────────────────

public class LabelVelocity
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("openedCount")]
    public int OpenedCount { get; set; }

    [JsonPropertyName("closedCount")]
    public int ClosedCount { get; set; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("netFlow")]
    public int NetFlow { get; set; }
}

public class LabelVelocityResult
{
    [JsonPropertyName("labels")]
    public List<LabelVelocity> Labels { get; set; } = new();

    [JsonPropertyName("lookbackDays")]
    public int LookbackDays { get; set; } = 30;
}

// ── Commit Leaders ───────────────────────────────────────────────────────────

public class CommitLeader
{
    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    [JsonPropertyName("commitCount")]
    public int CommitCount { get; set; }

    [JsonPropertyName("activeDays")]
    public int ActiveDays { get; set; }

    [JsonPropertyName("repos")]
    public int Repos { get; set; }
}

public class CommitLeadersResult
{
    [JsonPropertyName("leaders")]
    public List<CommitLeader> Leaders { get; set; } = new();

    [JsonPropertyName("totalCommits")]
    public int TotalCommits { get; set; }

    [JsonPropertyName("lookbackDays")]
    public int LookbackDays { get; set; } = 30;
}

// ── Issue Assignee Workload ──────────────────────────────────────────────────

public class AssigneeStat
{
    [JsonPropertyName("assignee")]
    public string Assignee { get; set; } = string.Empty;

    [JsonPropertyName("openCount")]
    public int OpenCount { get; set; }

    [JsonPropertyName("oldestDays")]
    public int OldestDays { get; set; }

    [JsonPropertyName("avgDays")]
    public int AvgDays { get; set; }
}

public class IssueAssigneesResult
{
    [JsonPropertyName("assignees")]
    public List<AssigneeStat> Assignees { get; set; } = new();

    [JsonPropertyName("totalAssignees")]
    public int TotalAssignees { get; set; }
}

// ── Milestone Progress ───────────────────────────────────────────────────────

public class MilestoneStat
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("openCount")]
    public int OpenCount { get; set; }

    [JsonPropertyName("closedCount")]
    public int ClosedCount { get; set; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("progressPct")]
    public int ProgressPct { get; set; }

    [JsonPropertyName("predictedDate")]
    public string? PredictedDate { get; set; }
}

public class MilestonesResult
{
    [JsonPropertyName("milestones")]
    public List<MilestoneStat> Milestones { get; set; } = new();

    [JsonPropertyName("totalMilestones")]
    public int TotalMilestones { get; set; }
}

public class WeekStat
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = "";

    [JsonPropertyName("thisWeek")]
    public int ThisWeek { get; set; }

    [JsonPropertyName("lastWeek")]
    public int LastWeek { get; set; }

    [JsonPropertyName("delta")]
    public int Delta { get; set; }

    [JsonPropertyName("trend")]
    public string Trend { get; set; } = "flat";
}

public class WeekOverWeekResult
{
    [JsonPropertyName("prs")]
    public WeekStat Prs { get; set; } = new();

    [JsonPropertyName("issues")]
    public WeekStat Issues { get; set; } = new();

    [JsonPropertyName("commits")]
    public WeekStat Commits { get; set; } = new();
}

public class TriageIssue
{
    [JsonPropertyName("issueNumber")]
    public string IssueNumber { get; set; } = "";

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("repo")]
    public string? Repo { get; set; }

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("ageHours")]
    public int AgeHours { get; set; }

    [JsonPropertyName("missing")]
    public List<string> Missing { get; set; } = new();
}

public class IssueTriageResult
{
    [JsonPropertyName("issues")]
    public List<TriageIssue> Issues { get; set; } = new();

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("lookbackDays")]
    public int LookbackDays { get; set; }
}

public class IssueFlowDay
{
    [JsonPropertyName("day")]
    public string Day { get; set; } = "";

    [JsonPropertyName("opened")]
    public int Opened { get; set; }

    [JsonPropertyName("closed")]
    public int Closed { get; set; }

    [JsonPropertyName("net")]
    public int Net { get; set; }
}

public class IssueFlowResult
{
    [JsonPropertyName("data")]
    public List<IssueFlowDay> Data { get; set; } = new();

    [JsonPropertyName("totalOpened")]
    public int TotalOpened { get; set; }

    [JsonPropertyName("totalClosed")]
    public int TotalClosed { get; set; }

    [JsonPropertyName("netFlow")]
    public int NetFlow { get; set; }

    [JsonPropertyName("lookbackDays")]
    public int LookbackDays { get; set; }
}

/// <summary>Cycle time stats for a single GitHub label.</summary>
public class CycleTimeByLabel
{
    [JsonPropertyName("label")] public string Label { get; set; } = "";
    [JsonPropertyName("issueCount")] public int IssueCount { get; set; }
    [JsonPropertyName("avgDays")] public double AvgDays { get; set; }
    [JsonPropertyName("medianDays")] public double MedianDays { get; set; }
    [JsonPropertyName("minDays")] public double MinDays { get; set; }
    [JsonPropertyName("maxDays")] public double MaxDays { get; set; }
}

/// <summary>Issue cycle time by label — average/median days from open to close.</summary>
public class IssueCycleTimeResult
{
    [JsonPropertyName("byLabel")] public List<CycleTimeByLabel> ByLabel { get; set; } = new();
    [JsonPropertyName("lookbackDays")] public int LookbackDays { get; set; }
}

/// <summary>One week of issue throughput data.</summary>
public class IssueThroughputWeek
{
    [JsonPropertyName("weekStart")] public string WeekStart { get; set; } = "";
    [JsonPropertyName("closedCount")] public int ClosedCount { get; set; }
    [JsonPropertyName("openedCount")] public int OpenedCount { get; set; }
}

/// <summary>Weekly issue throughput trend.</summary>
public class IssueThroughputResult
{
    [JsonPropertyName("weeks")] public List<IssueThroughputWeek> Weeks { get; set; } = new();
    [JsonPropertyName("avgClosedPerWeek")] public double AvgClosedPerWeek { get; set; }
    [JsonPropertyName("trend")] public string Trend { get; set; } = "stable";
    [JsonPropertyName("lookbackWeeks")] public int LookbackWeeks { get; set; }
}

// ── Issue Resolver Leaderboard (Phase 4) ─────────────────────────────────────

/// <summary>One entry in the issue resolver leaderboard.</summary>
public class IssueResolver
{
    [JsonPropertyName("login")] public string Login { get; set; } = "";
    [JsonPropertyName("closedCount")] public int ClosedCount { get; set; }
    [JsonPropertyName("pct")] public double Pct { get; set; }
}

/// <summary>Issue resolver leaderboard — top contributors by closed issue count.</summary>
public class IssueResolversResult
{
    [JsonPropertyName("resolvers")] public List<IssueResolver> Resolvers { get; set; } = new();
    [JsonPropertyName("totalClosed")] public int TotalClosed { get; set; }
    [JsonPropertyName("lookbackDays")] public int LookbackDays { get; set; }
}

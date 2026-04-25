using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Resource for GitHub project analytics (ADR-152).
/// Provides access to code intelligence, activity, and quality metrics for GitHub-connected projects.
/// </summary>
public class GitHubResource : BaseResource
{
    internal GitHubResource(HttpPipeline pipeline) : base(pipeline)
    {
    }

    private static string Esc(string value) => Uri.EscapeDataString(value);

    /// <summary>
    /// Lists GitHub repositories connected to a project.
    /// </summary>
    public virtual async Task<GitHubConnectionsResponse> ListConnectionsAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<GitHubConnectionsResponse>(
            $"/v1/projects/{Esc(projectId)}/github",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns recent GitHub activity memories for a project (commits, PRs, issues).
    /// </summary>
    public virtual async Task<GitHubActivityResponse> GetActivityAsync(
        string projectId,
        string? type = null,
        int? limit = null,
        int? offset = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        var queryParams = BuildQueryParams(
            ("type", type),
            ("limit", limit),
            ("offset", offset));
        return await GetAsync<GitHubActivityResponse>(
            $"/v1/projects/{Esc(projectId)}/github/activity",
            queryParams: queryParams,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns cyclomatic and cognitive complexity metrics for tracked files, sorted by hotspot score.
    /// Optionally filtered by a specific file path or repository.
    /// </summary>
    public virtual async Task<FileComplexityResponse> GetFileComplexityAsync(
        string projectId,
        string? filePath = null,
        string? repo = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        var queryParams = BuildQueryParams(
            ("file", filePath),
            ("repo", repo));
        return await GetAsync<FileComplexityResponse>(
            $"/v1/projects/{Esc(projectId)}/github/complexity",
            queryParams: queryParams,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns PR merge velocity metrics for a project.
    /// </summary>
    public virtual async Task<PRVelocityResponse> GetVelocityAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<PRVelocityResponse>(
            $"/v1/projects/{Esc(projectId)}/github/velocity",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns pull requests flagged with risk signals (scope creep, semantic drift, hotspots).
    /// </summary>
    public virtual async Task<FlaggedPRsResponse> GetFlaggedPRsAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<FlaggedPRsResponse>(
            $"/v1/projects/{Esc(projectId)}/github/flagged-prs",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns a one-call project health snapshot for agents.
    /// Aggregates code quality gate, PR velocity, open PR risk count, and top issues.
    /// </summary>
    public virtual async Task<HealthSnapshotResponse> GetHealthSnapshotAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<HealthSnapshotResponse>(
            $"/v1/projects/{Esc(projectId)}/github/health-snapshot",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns PR pre-review briefs with quality scores and risk signals.
    /// </summary>
    public virtual async Task<PRBriefsResponse> GetPrBriefsAsync(
        string projectId,
        string state = "open",
        int? prNumber = null,
        int limit = 20,
        int? minQualityScore = null,
        int? maxQualityScore = null,
        string? agent = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        var qs = $"state={Uri.EscapeDataString(state)}&limit={limit}";
        if (prNumber.HasValue)
            qs += $"&pr_number={prNumber.Value}";
        if (minQualityScore.HasValue)
            qs += $"&min_quality_score={minQualityScore.Value}";
        if (maxQualityScore.HasValue)
            qs += $"&max_quality_score={maxQualityScore.Value}";
        if (!string.IsNullOrEmpty(agent))
            qs += $"&agent={Uri.EscapeDataString(agent)}";
        return await GetAsync<PRBriefsResponse>(
            $"/v1/projects/{Esc(projectId)}/github/pr-briefs?{qs}",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns issue cycle time trends for a project.
    /// </summary>
    public virtual async Task<CycleTimeResponse> GetCycleTimeAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<CycleTimeResponse>(
            $"/v1/projects/{Esc(projectId)}/github/cycle-time",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns AI vs human commit and PR attribution for a project.
    /// </summary>
    public virtual async Task<AgentAttributionResponse> GetAgentAttributionAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<AgentAttributionResponse>(
            $"/v1/projects/{Esc(projectId)}/github/agent-attribution",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Generates a delivery narrative from recent GitHub activity.
    /// Returns a bullet-point summary of merged PRs, closed issues, and goal progress.
    /// </summary>
    public virtual async Task<GenerateNarrativeResponse> GenerateNarrativeAsync(
        string projectId,
        int windowDays = 7,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await PostAsync<GenerateNarrativeResponse>(
            $"/v1/projects/{Esc(projectId)}/github/narrative",
            new { window_days = windowDays },
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns goal progress driven by GitHub issue completions.
    /// Goals are updated automatically when linked issues are closed by merged PRs.
    /// </summary>
    public virtual async Task<GoalProgressResponse> GetGoalProgressAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<GoalProgressResponse>(
            $"/v1/projects/{Esc(projectId)}/github/goal-progress",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns a chronological feed of GitHub-driven goal progress events
    /// (PR merges → issue closures → goal bumps).
    /// </summary>
    public virtual async Task<GoalProgressHistoryResponse> GetGoalProgressHistoryAsync(
        string projectId,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<GoalProgressHistoryResponse>(
            $"/v1/projects/{Esc(projectId)}/github/goal-progress-history?limit={limit}",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns a 0–100 release readiness score from open PRs, blocking tasks,
    /// goal completion, and scope-creep events.
    /// </summary>
    public virtual async Task<ReleaseReadinessResponse> GetReleaseReadinessAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<ReleaseReadinessResponse>(
            $"/v1/projects/{Esc(projectId)}/github/release-readiness",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Fetches the most recently stored delivery narrative for a project.
    /// Returns null if no narrative has been generated yet.
    /// </summary>
    public virtual async Task<LatestNarrativeResponse> GetLatestNarrativeAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<LatestNarrativeResponse>(
            $"/v1/projects/{Esc(projectId)}/github/narrative",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates sync settings for a GitHub project connection.
    /// </summary>
    public virtual async Task<GitHubProjectConnection> UpdateConnectionAsync(
        string projectId,
        string connectionId,
        UpdateGitHubConnectionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        ArgumentException.ThrowIfNullOrEmpty(connectionId);
        ArgumentNullException.ThrowIfNull(request);
        return await PatchAsync<GitHubProjectConnection>(
            $"/v1/projects/{Esc(projectId)}/github/{Esc(connectionId)}",
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a GitHub repository connection from a project.
    /// </summary>
    public virtual async Task DeleteConnectionAsync(
        string projectId,
        string connectionId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        ArgumentException.ThrowIfNullOrEmpty(connectionId);
        await DeleteAsync(
            $"/v1/projects/{Esc(projectId)}/github/{Esc(connectionId)}",
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Triggers a full backfill scan of a linked GitHub repository.
    /// Fetches recent commits, PRs, and issues from GitHub, stores them as memories,
    /// runs complexity analysis, and generates PR pre-review briefs.
    /// </summary>
    public virtual async Task<ScanRepoResponse> ScanRepoAsync(
        string projectId,
        string connectionId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        ArgumentException.ThrowIfNullOrEmpty(connectionId);
        return await PostAsync<ScanRepoResponse>(
            $"/v1/projects/{Esc(projectId)}/github/{Esc(connectionId)}/scan",
            new { },
            cancellationToken).ConfigureAwait(false);
    }

    // ── Phase 5: Code Quality Scanner + Repo Stats ────────────────────────

    /// <summary>
    /// Triggers a code quality scan using GitHub security APIs (Dependabot, code scanning,
    /// secret scanning) plus an LLM enrichment pass over complexity and churn context.
    /// </summary>
    public virtual async Task<ImprovementGenerateResponse> GenerateCodeImprovementsAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await PostAsync<ImprovementGenerateResponse>(
            $"/v1/projects/{Esc(projectId)}/github/improvements/generate",
            new { },
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Lists code improvement suggestions with optional category, priority, and status filters.
    /// </summary>
    public virtual async Task<CodeImprovementsResponse> GetCodeImprovementsAsync(
        string projectId,
        string status = "open",
        string? category = null,
        string? priority = null,
        string? filePath = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        var queryParams = BuildQueryParams(
            ("status", status),
            ("category", category),
            ("priority", priority),
            ("file_path", filePath));
        return await GetAsync<CodeImprovementsResponse>(
            $"/v1/projects/{Esc(projectId)}/github/improvements",
            queryParams: queryParams,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Pushes a code quality finding to GitHub Issues and marks it in_progress.
    /// </summary>
    public virtual async Task<CreateIssueFromSuggestionResponse> CreateIssueFromSuggestionAsync(
        string projectId,
        string suggestionId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        ArgumentException.ThrowIfNullOrEmpty(suggestionId);
        return await PostAsync<CreateIssueFromSuggestionResponse>(
            $"/v1/projects/{Esc(projectId)}/github/improvements/{Esc(suggestionId)}/create-issue",
            body: null,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates the status of a code improvement suggestion.
    /// </summary>
    public virtual async Task<CodeImprovement> UpdateCodeImprovementStatusAsync(
        string projectId,
        string suggestionId,
        string status,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        ArgumentException.ThrowIfNullOrEmpty(suggestionId);
        var wrapper = await PatchAsync<ImprovementStatusWrapper>(
            $"/v1/projects/{Esc(projectId)}/github/improvements/{Esc(suggestionId)}",
            new { status },
            cancellationToken).ConfigureAwait(false);
        return wrapper.Suggestion;
    }

    /// <summary>
    /// Returns priority/category summary counts for open code improvement suggestions.
    /// </summary>
    public virtual async Task<List<ImprovementSummaryRow>> GetImprovementsSummaryAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        var wrapper = await GetAsync<ImprovementSummaryWrapper>(
            $"/v1/projects/{Esc(projectId)}/github/improvements/summary",
            cancellationToken: cancellationToken).ConfigureAwait(false);
        return wrapper.Summary;
    }

    /// <summary>
    /// Returns historical code quality metric snapshots for trend analysis.
    /// </summary>
    public virtual async Task<List<ImprovementsHistoryItem>> GetImprovementsHistoryAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        var wrapper = await GetAsync<ImprovementsHistoryWrapper>(
            $"/v1/projects/{Esc(projectId)}/github/improvements/history",
            cancellationToken: cancellationToken).ConfigureAwait(false);
        return wrapper.History;
    }

    /// <summary>
    /// Fetches live repo stats: stars, languages, contributors, LOC, README preview.
    /// </summary>
    public virtual async Task<RepoStatsResponse> GetRepoStatsAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<RepoStatsResponse>(
            $"/v1/projects/{Esc(projectId)}/github/improvements/stats",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Get technical debt aggregated by category.</summary>
    public virtual async Task<TechnicalDebt> GetCodeDebtAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<TechnicalDebt>(
            $"/v1/projects/{Esc(projectId)}/github/improvements/debt",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Evaluate quality gate thresholds — returns pass/fail with per-check detail.</summary>
    public virtual async Task<QualityGate> GetQualityGateAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<QualityGate>(
            $"/v1/projects/{Esc(projectId)}/github/improvements/quality-gate",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Run an agent PR review, posting a structured comment to GitHub.</summary>
    public virtual async Task<PrReviewResult> ReviewPrAsync(
        string projectId,
        string connectionId,
        int prNumber,
        string reviewEvent = "COMMENT",
        bool dryRun = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        ArgumentException.ThrowIfNullOrEmpty(connectionId);
        return await PostAsync<PrReviewResult>(
            $"/v1/projects/{Esc(projectId)}/github/review-pr",
            new { connection_id = connectionId, pr_number = prNumber, @event = reviewEvent, dry_run = dryRun },
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Create a GitHub PR with agent-authored file changes (up to 50 files).</summary>
    public virtual async Task<AgentPrResult> CreatePrAsync(
        string projectId,
        string connectionId,
        string branchName,
        string commitMessage,
        string prTitle,
        IEnumerable<PrFileChange> changes,
        string baseBranch = "main",
        string prBody = "",
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await PostAsync<AgentPrResult>(
            $"/v1/projects/{Esc(projectId)}/github/create-pr",
            new { connection_id = connectionId, branch_name = branchName, base_branch = baseBranch, commit_message = commitMessage, pr_title = prTitle, pr_body = prBody, changes },
            cancellationToken).ConfigureAwait(false);
    }

    // ── Code Health Panels ────────────────────────────────────────────────────

    /// <summary>High-level code health summary: LOC, complexity, maintainability, hotspot counts.</summary>
    public virtual async Task<CodeSummaryResult> GetCodeSummaryAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<CodeSummaryResult>(
            $"/v1/projects/{Esc(projectId)}/github/code-summary",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Structural clone groups — functions duplicated across files.</summary>
    public virtual async Task<CloneGroupsResult> GetCloneGroupsAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<CloneGroupsResult>(
            $"/v1/projects/{Esc(projectId)}/github/clone-groups",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Dead exports — exported symbols never imported elsewhere in the repo.</summary>
    public virtual async Task<DeadExportsResult> GetDeadExportsAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<DeadExportsResult>(
            $"/v1/projects/{Esc(projectId)}/github/dead-exports",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Test coverage by naming convention — source files paired with test files.</summary>
    public virtual async Task<TestCoverageResult> GetTestCoverageAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<TestCoverageResult>(
            $"/v1/projects/{Esc(projectId)}/github/test-coverage",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Load-bearing functions — functions called by many files; high blast radius.</summary>
    public virtual async Task<LoadBearingResult> GetLoadBearingFunctionsAsync(
        string projectId,
        int minCallers = 3,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<LoadBearingResult>(
            $"/v1/projects/{Esc(projectId)}/github/load-bearing?min_callers={minCallers}",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Bug density per file — open issues per thousand lines of code.</summary>
    public virtual async Task<BugDensityResult> GetBugDensityAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<BugDensityResult>(
            $"/v1/projects/{Esc(projectId)}/github/bug-density",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get the weekly PR quality score trend over the last 12 weeks.
    /// Returns one data point per week that had at least one reviewed PR.
    /// </summary>
    public virtual async Task<List<PRQualityWeek>> GetPrQualityTrendAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<List<PRQualityWeek>>(
            $"/v1/projects/{Esc(projectId)}/github/pr-quality-trend",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get active branches derived from commit memories with staleness detection.
    /// Branches with no commits in 14+ days are flagged as stale.
    /// </summary>
    public virtual async Task<ActiveBranchesResult> GetActiveBranchesAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<ActiveBranchesResult>(
            $"/v1/projects/{Esc(projectId)}/github/branches",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Per-contributor PR quality stats — avg score, test coverage %, PR count, last-active date.
    /// </summary>
    public virtual async Task<ContributorQualityResult> GetContributorQualityAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<ContributorQualityResult>(
            $"/v1/projects/{Esc(projectId)}/github/contributor-quality",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Open PRs sorted oldest-first with ageDays and isStale flag (>7 days without update).
    /// </summary>
    public virtual async Task<PrAgingResult> GetPrAgingAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<PrAgingResult>(
            $"/v1/projects/{Esc(projectId)}/github/pr-aging",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<PrSizeDistributionResult> GetPrSizeDistributionAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<PrSizeDistributionResult>(
            $"/v1/projects/{Esc(projectId)}/github/pr-size-distribution",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<ReviewTurnaroundResult> GetReviewTurnaroundAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<ReviewTurnaroundResult>(
            $"/v1/projects/{Esc(projectId)}/github/review-turnaround",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<WorkQueueResult> GetWorkQueueAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<WorkQueueResult>(
            $"/v1/projects/{Esc(projectId)}/github/work-queue",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<ReviewerWorkloadResult> GetReviewerWorkloadAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<ReviewerWorkloadResult>(
            $"/v1/projects/{Esc(projectId)}/github/reviewer-workload",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<ApprovedPRsResult> GetApprovedPRsAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<ApprovedPRsResult>(
            $"/v1/projects/{Esc(projectId)}/github/approved-prs",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<IssueBacklogResult> GetIssueBacklogAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<IssueBacklogResult>(
            $"/v1/projects/{Esc(projectId)}/github/issue-backlog",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<ReviewCoverageResult> GetReviewCoverageAsync(
        string projectId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectId);
        return await GetAsync<ReviewCoverageResult>(
            $"/v1/projects/{Esc(projectId)}/github/review-coverage",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}

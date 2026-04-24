using System.Text.Json.Serialization;

namespace Trix.Models;

// ── Code Analysis & PR Review Models ─────────────────────────────────────────

/// <summary>Technical debt aggregated by category.</summary>
public class TechnicalDebt
{
    [JsonPropertyName("totalMinutes")]
    public int TotalMinutes { get; set; }

    [JsonPropertyName("categories")]
    public List<DebtCategory> Categories { get; set; } = [];
}

/// <summary>A debt category with minute estimate.</summary>
public class DebtCategory
{
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("minutes")]
    public int Minutes { get; set; }
}

/// <summary>Quality gate result with per-check detail.</summary>
public class QualityGate
{
    [JsonPropertyName("passed")]
    public bool Passed { get; set; }

    [JsonPropertyName("checks")]
    public List<QualityCheck> Checks { get; set; } = [];
}

/// <summary>A single quality gate check result.</summary>
public class QualityCheck
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("passed")]
    public bool Passed { get; set; }

    [JsonPropertyName("value")]
    public double? Value { get; set; }

    [JsonPropertyName("threshold")]
    public double? Threshold { get; set; }
}

/// <summary>A dependency vulnerability found in a changed manifest.</summary>
public class DepVuln
{
    [JsonPropertyName("priority")]
    public string Priority { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("file_path")]
    public string? FilePath { get; set; }

    [JsonPropertyName("evidence")]
    public DepVulnEvidence Evidence { get; set; } = new();
}

/// <summary>Evidence fields for a dependency vulnerability.</summary>
public class DepVulnEvidence
{
    [JsonPropertyName("vuln_id")]
    public string VulnId { get; set; } = string.Empty;

    [JsonPropertyName("package")]
    public string Package { get; set; } = string.Empty;

    [JsonPropertyName("ecosystem")]
    public string Ecosystem { get; set; } = string.Empty;

    [JsonPropertyName("cvss")]
    public double? Cvss { get; set; }

    [JsonPropertyName("fix_version")]
    public string? FixVersion { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>A security finding from SAST or secret scanning.</summary>
public class SecurityFinding
{
    [JsonPropertyName("priority")]
    public string Priority { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("file_path")]
    public string? FilePath { get; set; }

    [JsonPropertyName("generated_by")]
    public string GeneratedBy { get; set; } = string.Empty;
}

/// <summary>Per-file metrics for a changed file in a PR review.</summary>
public class PrFileMetric
{
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("cc")]
    public int? Cc { get; set; }

    [JsonPropertyName("cogc")]
    public int? Cogc { get; set; }

    [JsonPropertyName("mi")]
    public int? Mi { get; set; }

    [JsonPropertyName("loc")]
    public int? Loc { get; set; }

    [JsonPropertyName("testCoverage")]
    public Dictionary<string, object>? TestCoverage { get; set; }

    [JsonPropertyName("unusedExports")]
    public List<string> UnusedExports { get; set; } = [];
}

/// <summary>Full PR review result from the Trix agent reviewer.</summary>
public class PrReviewResult
{
    [JsonPropertyName("review")]
    public PrReviewBody Review { get; set; } = new();

    [JsonPropertyName("signals")]
    public List<object> Signals { get; set; } = [];

    [JsonPropertyName("smells")]
    public List<object> Smells { get; set; } = [];

    [JsonPropertyName("securityFindings")]
    public List<SecurityFinding> SecurityFindings { get; set; } = [];

    [JsonPropertyName("depVulns")]
    public List<DepVuln> DepVulns { get; set; } = [];

    [JsonPropertyName("fileMetrics")]
    public List<PrFileMetric> FileMetrics { get; set; } = [];

    [JsonPropertyName("inlineComments")]
    public int InlineComments { get; set; }

    [JsonPropertyName("filesAnalyzed")]
    public int FilesAnalyzed { get; set; }

    [JsonPropertyName("posted")]
    public bool Posted { get; set; }
}

/// <summary>Review body and metadata.</summary>
public class PrReviewBody
{
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("event")]
    public string Event { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>Result from agent-authored PR creation.</summary>
public class AgentPrResult
{
    [JsonPropertyName("prNumber")]
    public int PrNumber { get; set; }

    [JsonPropertyName("prUrl")]
    public string PrUrl { get; set; } = string.Empty;

    [JsonPropertyName("branchName")]
    public string BranchName { get; set; } = string.Empty;

    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;
}

/// <summary>A file change for agent-authored PR creation.</summary>
public class PrFileChange
{
    [JsonPropertyName("file_path")]
    public string FilePath { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

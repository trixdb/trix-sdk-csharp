namespace Trix.Models;

/// <summary>
/// Represents a custom hub role (ADR-080).
/// </summary>
public class HubRole
{
    public string Id { get; set; } = string.Empty;
    public string HubId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public int Position { get; set; }
    public Dictionary<string, string> Permissions { get; set; } = new();
    public bool IsDefault { get; set; }
}

/// <summary>
/// Request to create a hub role.
/// </summary>
public class CreateHubRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public int? Position { get; set; }
    public Dictionary<string, string>? Permissions { get; set; }
}

/// <summary>
/// Request to update a hub role.
/// </summary>
public class UpdateHubRoleRequest
{
    public string? Name { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public int? Position { get; set; }
    public Dictionary<string, string>? Permissions { get; set; }
}

/// <summary>
/// Request to assign a role to a user.
/// </summary>
public class AssignRoleRequest
{
    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// Represents a per-conversation role permission override.
/// </summary>
public class ConversationRoleOverride
{
    public string Id { get; set; } = string.Empty;
    public string ConversationId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public Dictionary<string, string> Permissions { get; set; } = new();
}

namespace Trix.Models;

/// <summary>
/// Represents a calendar event (ADR-075).
/// </summary>
public class CalendarEvent
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public CalendarEventTime Start { get; set; } = new();
    public CalendarEventTime End { get; set; } = new();
    public string? Location { get; set; }
    public string? Description { get; set; }
    public int? AttendeeCount { get; set; }
    public string CalendarId { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}

/// <summary>
/// Start or end time for a calendar event.
/// </summary>
public class CalendarEventTime
{
    public string? DateTime { get; set; }
    public string? Date { get; set; }
    public string? TimeZone { get; set; }
}

/// <summary>
/// Response for listing calendar events.
/// </summary>
public class CalendarEventsResponse
{
    public List<CalendarEvent> Events { get; set; } = new();
    public int Total { get; set; }
    public bool? HasMore { get; set; }
}

/// <summary>
/// Result of syncing calendar events to memories.
/// </summary>
public class CalendarSyncResponse
{
    public int Synced { get; set; }
    public int Skipped { get; set; }
    public int Failed { get; set; }
    public int TotalEvents { get; set; }
}

/// <summary>
/// A calendar provider connection.
/// </summary>
public class CalendarConnection
{
    public string Id { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ConnectedAt { get; set; } = string.Empty;
    public string? LastSyncAt { get; set; }
}

/// <summary>
/// Response for listing calendar connections.
/// </summary>
public class CalendarConnectionsResponse
{
    public List<CalendarConnection> Connections { get; set; } = new();
}

/// <summary>
/// A calendar within a connection.
/// </summary>
public class CalendarInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool? Primary { get; set; }
    public bool? SyncEnabled { get; set; }
}

/// <summary>
/// Response for listing calendars.
/// </summary>
public class CalendarListResponse
{
    public List<CalendarInfo> Calendars { get; set; } = new();
    public string Provider { get; set; } = string.Empty;
}

/// <summary>
/// Request to sync calendar events to memories.
/// </summary>
public class SyncCalendarRequest
{
    public string ConnectionId { get; set; } = string.Empty;
    public string? CalendarId { get; set; }
    public int? DaysPast { get; set; }
    public int? DaysFuture { get; set; }
    public string? SpaceId { get; set; }
    public string? PiiLevel { get; set; }
    public List<string>? Tags { get; set; }
}

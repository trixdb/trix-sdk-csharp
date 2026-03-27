using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// Represents a calendar event (ADR-075).
/// </summary>
public class CalendarEvent
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("start")]
    public CalendarEventTime Start { get; set; } = new();

    [JsonPropertyName("end")]
    public CalendarEventTime End { get; set; } = new();

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("attendee_count")]
    public int? AttendeeCount { get; set; }

    [JsonPropertyName("calendar_id")]
    public string CalendarId { get; set; } = string.Empty;

    [JsonPropertyName("provider")]
    public string Provider { get; set; } = string.Empty;
}

/// <summary>
/// Start or end time for a calendar event.
/// </summary>
public class CalendarEventTime
{
    [JsonPropertyName("dateTime")]
    public string? DateTime { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("timeZone")]
    public string? TimeZone { get; set; }
}

/// <summary>
/// Response for listing calendar events.
/// </summary>
public class CalendarEventsResponse
{
    [JsonPropertyName("events")]
    public List<CalendarEvent> Events { get; set; } = new();

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }
}

/// <summary>
/// Result of syncing calendar events to memories.
/// </summary>
public class CalendarSyncResponse
{
    [JsonPropertyName("synced")]
    public int Synced { get; set; }

    [JsonPropertyName("skipped")]
    public int Skipped { get; set; }

    [JsonPropertyName("failed")]
    public int Failed { get; set; }

    [JsonPropertyName("total_events")]
    public int TotalEvents { get; set; }
}

/// <summary>
/// A calendar provider connection.
/// </summary>
public class CalendarConnection
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("provider")]
    public string Provider { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("connected_at")]
    public string ConnectedAt { get; set; } = string.Empty;

    [JsonPropertyName("last_sync_at")]
    public string? LastSyncAt { get; set; }
}

/// <summary>
/// Response for listing calendar connections.
/// </summary>
public class CalendarConnectionsResponse
{
    [JsonPropertyName("connections")]
    public List<CalendarConnection> Connections { get; set; } = new();
}

/// <summary>
/// A calendar within a connection.
/// </summary>
public class CalendarInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("primary")]
    public bool? Primary { get; set; }

    [JsonPropertyName("sync_enabled")]
    public bool? SyncEnabled { get; set; }
}

/// <summary>
/// Response for listing calendars.
/// </summary>
public class CalendarListResponse
{
    [JsonPropertyName("calendars")]
    public List<CalendarInfo> Calendars { get; set; } = new();

    [JsonPropertyName("provider")]
    public string Provider { get; set; } = string.Empty;
}

/// <summary>
/// Request to sync calendar events to memories.
/// </summary>
public class SyncCalendarRequest
{
    [JsonPropertyName("connection_id")]
    public string ConnectionId { get; set; } = string.Empty;

    [JsonPropertyName("calendar_id")]
    public string? CalendarId { get; set; }

    [JsonPropertyName("days_past")]
    public int? DaysPast { get; set; }

    [JsonPropertyName("days_future")]
    public int? DaysFuture { get; set; }

    [JsonPropertyName("space_id")]
    public string? SpaceId { get; set; }

    [JsonPropertyName("pii_level")]
    public string? PiiLevel { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }
}

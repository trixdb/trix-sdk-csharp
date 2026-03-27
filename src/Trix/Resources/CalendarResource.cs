using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Resource for calendar operations (ADR-075).
/// </summary>
public class CalendarResource : BaseResource
{
    internal CalendarResource(HttpPipeline pipeline) : base(pipeline)
    {
    }

    /// <summary>
    /// Lists upcoming calendar events.
    /// </summary>
    public virtual async Task<CalendarEventsResponse> ListEventsAsync(
        string? connectionId = null,
        string? calendarId = null,
        int? days = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = BuildQueryParams(
            ("connection_id", connectionId),
            ("calendar_id", calendarId),
            ("days", days),
            ("limit", limit));
        return await GetAsync<CalendarEventsResponse>(
            "/v1/calendar/events",
            queryParams: queryParams,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Syncs calendar events to memories.
    /// </summary>
    public virtual async Task<CalendarSyncResponse> SyncToMemoriesAsync(
        SyncCalendarRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await PostAsync<CalendarSyncResponse>(
            "/v1/calendar/sync",
            request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Lists calendar connections.
    /// </summary>
    public virtual async Task<CalendarConnectionsResponse> ListConnectionsAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<CalendarConnectionsResponse>(
            "/v1/calendar/connections",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Lists calendars for a connection.
    /// </summary>
    public virtual async Task<CalendarListResponse> ListCalendarsAsync(
        string connectionId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionId);
        return await GetAsync<CalendarListResponse>(
            $"/v1/calendar/connections/{Esc(connectionId)}/calendars",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private static string Esc(string value) => Uri.EscapeDataString(value);
}

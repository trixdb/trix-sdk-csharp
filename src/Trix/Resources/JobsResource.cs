using System.Runtime.CompilerServices;
using Trix.Internal;
using Trix.Models;

namespace Trix.Resources;

/// <summary>
/// Resource for background job management.
/// </summary>
public class JobsResource : BaseResource
{
    /// <summary>
    /// Initializes a new instance of the JobsResource.
    /// </summary>
    internal JobsResource(HttpPipeline pipeline) : base(pipeline)
    {
    }

    /// <summary>
    /// Gets job queue statistics.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Job statistics.</returns>
    public virtual async Task<JobStats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<JobStats>("/v1/jobs/stats", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a specific job.
    /// </summary>
    /// <param name="queue">The queue name.</param>
    /// <param name="id">The job ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The job.</returns>
    public virtual async Task<Job> GetAsync(
        string queue,
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(queue);
        ArgumentException.ThrowIfNullOrEmpty(id);

        return await GetAsync<Job>($"/v1/jobs/{Uri.EscapeDataString(queue)}/{Uri.EscapeDataString(id)}", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Lists jobs.
    /// </summary>
    /// <param name="request">List parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of jobs.</returns>
    public virtual async Task<PaginatedResponse<Job>> ListAsync(
        ListJobsRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = BuildQueryParams(
            ("queue", request?.Queue),
            ("status", request?.Status?.ToString().ToLowerInvariant()),
            ("limit", request?.Limit),
            ("offset", request?.Offset)
        );

        return await GetAsync<PaginatedResponse<Job>>("/v1/jobs", queryParams, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Lists all jobs with automatic pagination.
    /// </summary>
    /// <param name="request">List parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of jobs.</returns>
    public virtual async IAsyncEnumerable<Job> ListAllAsync(
        ListJobsRequest? request = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var offset = 0;
        var limit = request?.Limit ?? 100;
        int pages = 0;
        const int MaxPages = 1000;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (++pages > MaxPages)
                throw new InvalidOperationException($"Pagination exceeded {MaxPages} pages");

            var response = await ListAsync(new ListJobsRequest
            {
                Queue = request?.Queue,
                Status = request?.Status,
                Limit = limit,
                Offset = offset
            }, cancellationToken).ConfigureAwait(false);

            foreach (var job in response.Data)
            {
                yield return job;
            }

            if (response.Pagination?.HasMore != true)
            {
                break;
            }

            offset += limit;
        }
    }

    /// <summary>
    /// Retries a failed job.
    /// </summary>
    /// <param name="queue">The queue name.</param>
    /// <param name="id">The job ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The retried job.</returns>
    public virtual async Task<Job> RetryAsync(
        string queue,
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(queue);
        ArgumentException.ThrowIfNullOrEmpty(id);

        return await PostAsync<Job>($"/v1/jobs/{Uri.EscapeDataString(queue)}/{Uri.EscapeDataString(id)}/retry", null, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a job.
    /// </summary>
    /// <param name="queue">The queue name.</param>
    /// <param name="id">The job ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public virtual async Task RemoveAsync(
        string queue,
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(queue);
        ArgumentException.ThrowIfNullOrEmpty(id);

        await base.DeleteAsync($"/v1/jobs/{Uri.EscapeDataString(queue)}/{Uri.EscapeDataString(id)}", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Cleans up old jobs.
    /// </summary>
    /// <param name="queue">The queue name.</param>
    /// <param name="status">Status of jobs to clean.</param>
    /// <param name="graceMs">Grace period in milliseconds.</param>
    /// <param name="limit">Maximum number of jobs to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The clean result.</returns>
    public virtual async Task<CleanJobsResult> CleanAsync(
        string queue,
        JobStatus? status = null,
        int? graceMs = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(queue);

        var request = new
        {
            status = status?.ToString().ToLowerInvariant(),
            grace = graceMs,
            limit
        };

        return await PostAsync<CleanJobsResult>($"/v1/jobs/{Uri.EscapeDataString(queue)}/clean", request, cancellationToken)
            .ConfigureAwait(false);
    }
}

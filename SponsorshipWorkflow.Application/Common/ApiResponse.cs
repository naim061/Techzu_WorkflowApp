using System.Net;

namespace SponsorshipWorkflow.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public PaginationMeta? Pagination { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success")
        => new()
        {
            Success = true,
            StatusCode = (int)HttpStatusCode.OK,
            Message = message,
            Data = data
        };

    public static ApiResponse<T> Created(T data, string message = "Created successfully")
        => new()
        {
            Success = true,
            StatusCode = (int)HttpStatusCode.Created,
            Message = message,
            Data = data
        };

    public static ApiResponse<T> Fail(
        string message,
        int statusCode = (int)HttpStatusCode.BadRequest,
        List<string>? errors = null)
        => new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Errors = errors
        };

    public static ApiResponse<T> NotFound(string message = "Resource not found")
        => new()
        {
            Success = false,
            StatusCode = (int)HttpStatusCode.NotFound,
            Message = message
        };

    public static ApiResponse<T> Unauthorized(string message = "Unauthorized")
        => new()
        {
            Success = false,
            StatusCode = (int)HttpStatusCode.Unauthorized,
            Message = message
        };

    public static ApiResponse<T> Forbidden(string message = "Access denied")
        => new()
        {
            Success = false,
            StatusCode = (int)HttpStatusCode.Forbidden,
            Message = message
        };

    public ApiResponse<T> WithPagination(PaginationMeta pagination)
    {
        Pagination = pagination;
        return this;
    }

    public ApiResponse<T> WithTraceId(string traceId)
    {
        TraceId = traceId;
        return this;
    }
}

public class PaginationMeta
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace SponsorshipWorkflow.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        ApiResponse<object> response;

        switch (exception)
        {
            case ValidationException ex:
                _logger.LogWarning("Validation error [{TraceId}]: {Message}", traceId, ex.Message);
                response = ApiResponse<object>.Fail(ex.Message, ex.StatusCode, ex.Errors);
                break;

            case NotFoundException ex:
                _logger.LogWarning("Not found [{TraceId}]: {Message}", traceId, ex.Message);
                response = ApiResponse<object>.NotFound(ex.Message);
                break;

            case UnauthorizedException ex:
                _logger.LogWarning("Unauthorized [{TraceId}]: {Message}", traceId, ex.Message);
                response = ApiResponse<object>.Unauthorized(ex.Message);
                break;

            case ForbiddenException ex:
                _logger.LogWarning("Forbidden [{TraceId}]: {Message}", traceId, ex.Message);
                response = ApiResponse<object>.Forbidden(ex.Message);
                break;

            case ConflictException ex:
                _logger.LogWarning("Conflict [{TraceId}]: {Message}", traceId, ex.Message);
                response = ApiResponse<object>.Fail(ex.Message, ex.StatusCode);
                break;

            case WorkflowException ex:
                _logger.LogWarning("Workflow error [{TraceId}]: {Message}", traceId, ex.Message);
                response = ApiResponse<object>.Fail(ex.Message, ex.StatusCode);
                break;

            case AppException ex:
                _logger.LogWarning("App error [{TraceId}]: {Message}", traceId, ex.Message);
                response = ApiResponse<object>.Fail(ex.Message, ex.StatusCode, ex.Errors);
                break;

            default:
                _logger.LogError(exception, "Unhandled exception [{TraceId}]", traceId);
                var message = _env.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred.";
                response = ApiResponse<object>.Fail(
                    message, (int)HttpStatusCode.InternalServerError,
                    _env.IsDevelopment()
                        ? new List<string> { exception.StackTrace ?? string.Empty }
                        : null);
                break;
        }

        response.TraceId = traceId;
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
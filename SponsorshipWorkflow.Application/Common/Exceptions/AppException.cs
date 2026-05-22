using System.Net;

namespace SponsorshipWorkflow.Application.Common.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; }
    public List<string>? Errors { get; }

    public AppException(string message, int statusCode = (int)HttpStatusCode.BadRequest)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public AppException(string message, List<string> errors, int statusCode = (int)HttpStatusCode.BadRequest)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message = "Resource not found")
        : base(message, (int)HttpStatusCode.NotFound) { }

    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with key '{key}' was not found.", (int)HttpStatusCode.NotFound) { }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Unauthorized access")
        : base(message, (int)HttpStatusCode.Unauthorized) { }
}

public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "You do not have permission to perform this action")
        : base(message, (int)HttpStatusCode.Forbidden) { }
}

public class ValidationException : AppException
{
    public ValidationException(List<string> errors)
        : base("Validation failed", errors, (int)HttpStatusCode.UnprocessableEntity) { }

    public ValidationException(string message)
        : base(message, (int)HttpStatusCode.UnprocessableEntity) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message)
        : base(message, (int)HttpStatusCode.Conflict) { }
}

public class WorkflowException : AppException
{
    public WorkflowException(string message)
        : base(message, (int)HttpStatusCode.BadRequest) { }
}
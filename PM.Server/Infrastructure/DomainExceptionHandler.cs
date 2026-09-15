using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PM.Core.Exceptions;

namespace PM.Server.Infrastructure
{
    /// <summary>
    /// Translates expected domain failures into RFC 9457 problem responses.
    /// Returns false for anything else so the next handler can deal with it.
    /// </summary>
    public sealed class DomainExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<DomainExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not DomainException domainException)
            {
                return false;
            }

            var (statusCode, title) = Map(domainException);

            // Expected failures are information, not errors - logging them at
            // Error level would drown out the defects that actually need action.
            logger.LogInformation(
                "{Method} {Path} rejected with {StatusCode}: {Message}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                statusCode,
                domainException.Message);

            httpContext.Response.StatusCode = statusCode;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = domainException,
                ProblemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = domainException.Message
                }
            });
        }

        private static (int StatusCode, string Title) Map(DomainException exception) => exception switch
        {
            InvalidRequestException => (StatusCodes.Status400BadRequest, "Invalid request"),
            NotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflicting state"),
            CapacityExceededException => (StatusCodes.Status503ServiceUnavailable, "No capacity available"),
            _ => (StatusCodes.Status500InternalServerError, "Unexpected error")
        };
    }
}

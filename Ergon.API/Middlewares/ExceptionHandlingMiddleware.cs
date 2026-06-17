using Ergon.Exceptions;
using Npgsql;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode statusCode;
            string message;

            switch (ex)
            {
                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = ex.Message;
                    break;
                case BadRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;
                case UnauthorizedException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = ex.Message;
                    break;
                case ForbiddenException:
                    statusCode = HttpStatusCode.Forbidden;
                    message = ex.Message;
                    break;
                case ConflictException:
                    statusCode = HttpStatusCode.Conflict;
                    message = ex.Message;
                    break;
                default:
                    (statusCode, message) = ResolveDbException(ex);
                    break;
            }

            var result = new { message };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }

        private static (HttpStatusCode statusCode, string message) ResolveDbException(Exception ex)
        {
            Exception? current = ex;
            PostgresException? postgresEx = null;

            while (current != null)
            {
                if (current is PostgresException pg)
                {
                    postgresEx = pg;
                    break;
                }
                current = current.InnerException;
            }

            if (postgresEx != null)
            {
                return postgresEx.SqlState switch
                {
                    "23505" => (HttpStatusCode.Conflict, "A record with this value already exists."),
                    "23503" => (HttpStatusCode.BadRequest, "Operation failed due to an invalid reference or a restrict constraint."),
                    "23502" => (HttpStatusCode.BadRequest, "A required field is missing a value."),
                    "23514" => (HttpStatusCode.BadRequest, "A database check constraint was violated."),
                    "40001" => (HttpStatusCode.Conflict, "A concurrent update conflict occurred. Please retry."),
                    "40P01" => (HttpStatusCode.Conflict, "A deadlock was detected. Please retry."),
                    "08006" => (HttpStatusCode.ServiceUnavailable, "Database connection failed."),
                    "57P01" => (HttpStatusCode.ServiceUnavailable, "The database server is shutting down."),
                    _ => (HttpStatusCode.InternalServerError, "An unexpected database error occurred.")
                };
            }

            return (HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }
}

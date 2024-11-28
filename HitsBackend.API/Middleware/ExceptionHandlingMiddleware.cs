using System.Net;
using System.Text.Json;
using HitsBackend.Application.Common.Exceptions;

namespace HitsBackend.Middleware;

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
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = new
        {
            Status = (int)GetStatusCode(exception),
            Title = GetTitle(exception),
            Message = GetMessage(exception)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.Status;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static HttpStatusCode GetStatusCode(Exception exception) =>
        exception switch
        {
            FluentValidation.ValidationException => HttpStatusCode.BadRequest,
            ValidationException => HttpStatusCode.BadRequest,
            NotFoundException => HttpStatusCode.NotFound,
            ConflictException => HttpStatusCode.Conflict,
            UnauthorizedException => HttpStatusCode.Unauthorized,
            ForbiddenException => HttpStatusCode.Forbidden,
            _ => HttpStatusCode.InternalServerError
        };

    private static string GetTitle(Exception exception) =>
        exception switch
        {
            FluentValidation.ValidationException => "Validation Error",
            ValidationException => "Validation Error",
            NotFoundException => "Not Found",
            ConflictException => "Conflict",
            UnauthorizedException => "Unauthorized",
            ForbiddenException => "Forbidden",
            _ => "Server Error"
        };

    private static string GetMessage(Exception exception) =>
        exception switch
        {
            FluentValidation.ValidationException validationException =>
                string.Join("; ", validationException.Errors.Select(x => x.ErrorMessage)),
            _ => exception.Message
        };
} 
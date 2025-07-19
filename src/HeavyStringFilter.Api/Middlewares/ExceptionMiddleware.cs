using System.Net;
using System.Text.Json;

namespace HeavyStringFilter.Api.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (InvalidOperationException ex)
        {
            await HandleExceptionAsync(context, ex, logger, HttpStatusCode.BadRequest, "Invalid operation occurred.");
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, logger, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        ILogger logger,
        HttpStatusCode statusCode,
        string title)
    {
        logger.LogError(exception, title);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var problem = new
        {
            title,
            status = context.Response.StatusCode,
            detail = exception.Message,
            traceId = context.TraceIdentifier
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(problem, options);

        await context.Response.WriteAsync(json);
    }
}

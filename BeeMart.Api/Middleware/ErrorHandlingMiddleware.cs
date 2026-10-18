using System.Text.Json;
using BeeMart.Api.Dtos;
using BeeMart.Api.Services;

namespace BeeMart.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
        catch (ValidationException ex)
        {
            await WriteAsync(context, 400, "VALIDATION_ERROR", ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            await WriteAsync(context, 401, ex.Code, ex.Message);
        }
        catch (ConflictException ex)
        {
            await WriteAsync(context, 409, ex.Code, ex.Message);
        }
        catch (NotFoundException ex)
        {
            await WriteAsync(context, 404, "NOT_FOUND", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error");
            await WriteAsync(context, 500, "INTERNAL_ERROR", "Terjadi kesalahan pada server.");
        }
    }

    private static async Task WriteAsync(HttpContext context, int status, string code, string message)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";
        var payload = new ErrorResponse(code, message, null);
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        await context.Response.WriteAsync(json);
    }
}

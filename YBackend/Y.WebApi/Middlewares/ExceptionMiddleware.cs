using System.ComponentModel.DataAnnotations;

namespace Y.WebApi.Middlewares;

public class ExceptionMiddleware
{

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch(ValidationException e)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            
            await context.Response.WriteAsync($"{nameof(ValidationException)} {e.Message}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error caught by exception middleware");
            context.Response.StatusCode = 500;

            await context.Response.WriteAsync("Internal server error");
        }
    }
}

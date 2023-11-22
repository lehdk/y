using System.ComponentModel.DataAnnotations;

namespace Y.WebApi.Middlewares;

public class ExceptionMiddleware
{

    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
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
        catch (Exception)
        {
            context.Response.StatusCode = 500;

            await context.Response.WriteAsync("Internal server error");
        }
    }
}

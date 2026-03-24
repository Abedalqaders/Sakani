using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

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
            await _next(context); // كمل الطلب بشكل طبيعي
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex); // إذا وقع خطأ، تعال هنا
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // هون بنحدد الـ Status Code بناءً على نوع الخطأ
        context.Response.StatusCode = exception switch
        {
            KeyNotFoundException => (int)HttpStatusCode.NotFound,      // 404
            UnauthorizedAccessException => (int)HttpStatusCode.Forbidden, // 403
            _ => (int)HttpStatusCode.InternalServerError              // 500
        };

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message,
            Detail = exception.StackTrace?.ToString()
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}
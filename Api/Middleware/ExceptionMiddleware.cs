using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env)
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
            // 1. تسجيل الخطأ في السيرفر عشان تقدر تراقبه
            _logger.LogError(ex, "An unhandled exception occurred during the request.");

            await HandleExceptionAsync(context, ex, _env);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, IHostEnvironment env)
    {
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = exception switch
        {
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedAccessException => (int)HttpStatusCode.Forbidden,
            System.ComponentModel.DataAnnotations.ValidationException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        // 2. إخفاء تفاصيل الخطأ الحساسة في بيئة الإنتاج
        var isDevelopment = env.IsDevelopment();
        var isServerError = context.Response.StatusCode == 500;

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            // إذا كان 500 وإحنا مش في بيئة التطوير، بنعطي رسالة عامة
            Message = isServerError && !isDevelopment
                        ? "An internal server error occurred. Please try again later."
                        : exception.Message,
            // الـ StackTrace بيطلع بس للمبرمج في بيئة التطوير
            Detail = isDevelopment ? exception.StackTrace?.ToString() : null
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}
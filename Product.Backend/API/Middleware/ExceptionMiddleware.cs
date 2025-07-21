using Microsoft.AspNetCore.Mvc;
using System.Net;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Product.Backend.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            ProblemDetails problem;

            switch (ex)
            {
                case BadHttpRequestException badHttpRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    problem = new ProblemDetails
                    {
                        Status = (int)statusCode,
                        Title = badHttpRequestException.Message,
                        Type = nameof(BadHttpRequestException),
                        Detail = badHttpRequestException.InnerException?.Message
                    };
                    break;
                case DbUpdateConcurrencyException concurrencyException:
                    statusCode = HttpStatusCode.Conflict;
                    problem = new ProblemDetails
                    {
                        Status = (int)statusCode,
                        Title = concurrencyException.Message,
                        Type = nameof(UnauthorizedAccessException),
                        Detail = concurrencyException.InnerException?.Message
                    };
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    problem = new ProblemDetails
                    {
                        Status = (int)statusCode,
                        Title = ex.Message,
                        Type = nameof(Exception),
                        Detail = ex.InnerException?.Message
                    };
                    break;
            }

            var logMessage = JsonConvert.SerializeObject(problem);
            _logger.LogError(logMessage);

            httpContext.Response.StatusCode = (int)statusCode;
            var response = new
            {
                StatusCode = statusCode,
                Message = "An unexpected error occurred. Please try again later."
            };
            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}

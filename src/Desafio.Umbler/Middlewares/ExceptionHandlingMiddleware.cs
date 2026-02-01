using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Desafio.Umbler.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var (statusCode, title, detail) = ex switch
                {
                    ArgumentException or ArgumentNullException => (
                        StatusCodes.Status400BadRequest,
                        "Bad Request",
                        ex.Message
                    ),
                    KeyNotFoundException => (
                        StatusCodes.Status404NotFound,
                        "Not Found",
                        ex.Message
                    ),
                    UnauthorizedAccessException => (
                        StatusCodes.Status401Unauthorized,
                        "Unauthorized",
                        "Authentication required"
                    ),
                    InvalidOperationException => (
                        StatusCodes.Status409Conflict,
                        "Conflict",
                        ex.Message
                    ),
                    _ => (
                        StatusCodes.Status500InternalServerError,
                        "Internal Server Error",
                        "An unexpected error occurred"
                    ),
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/problem+json";

                var problemDetails = new ProblemDetails
                {
                    Type = $"https://httpstatuses.com/{statusCode}",
                    Title = title,
                    Status = statusCode,
                    Detail = detail,
                    Instance = context.Request.Path
                };

                var result = JsonSerializer.Serialize(problemDetails);
                await context.Response.WriteAsync(result);
            }
        }
    }
}

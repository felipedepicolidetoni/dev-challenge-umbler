using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using System.Text.Json;

namespace Desafio.Umbler.Middlewares
{
    public class ValidationExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/problem+json";
                var problemDetails = new ProblemDetails
                {
                    Type = "https://httpstatuses.com/400",
                    Title = "Validation Error",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage)),
                    Instance = context.Request.Path
                };
                var result = JsonSerializer.Serialize(problemDetails);
                await context.Response.WriteAsync(result);
            }
        }
    }
}

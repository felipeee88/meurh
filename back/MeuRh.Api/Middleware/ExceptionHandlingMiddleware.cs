using System.Net;
using System.Text.Json;
using MeuRh.Api.Models;
using MeuRh.Domain.Exceptions;

namespace MeuRh.Api.Middleware;

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
            _logger.LogError(ex, "Exceção não tratada ocorreu durante o processamento da requisição");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            BusinessRuleException => HttpStatusCode.BadRequest,
            DomainException => HttpStatusCode.BadRequest,
            FluentValidation.ValidationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var message = GetMessage(exception);
        var apiResponse = ApiResponse<object>.Error(message);

        var json = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return context.Response.WriteAsync(json);
    }

    private static string GetMessage(Exception exception)
    {
        if (exception is FluentValidation.ValidationException validationException)
        {
            var firstError = validationException.Errors.FirstOrDefault();
            return firstError?.ErrorMessage ?? "Dados inválidos fornecidos";
        }

        return exception.Message;
    }
}


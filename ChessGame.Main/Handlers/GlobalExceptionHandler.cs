using System;
using System.Net;
using System.Runtime.CompilerServices;
using ChessGame.Main.Exceptions.InnerExceptions;
using ChessGame.Main.Exceptions.ResponseExceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ChessGame.Main.Handlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        HandleExceptionByLogger(exception);
        int status = GetStatusCode(exception);
        httpContext.Response.StatusCode = status;
        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = "An error occurred",
            Type = typeof(Exception).Name,
        };
        if (exception is ServerResponseException)
        {
            problemDetails.Type = exception.GetType().Name;
            problemDetails.Detail = exception.Message;
        }
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            InnerExceptionBase => StatusCodes.Status500InternalServerError,
            ServerResponseException => ((ServerResponseException)exception).Code,
            _ => StatusCodes.Status500InternalServerError,
        };

    private void HandleExceptionByLogger(Exception exception)
    {
        if (exception is ServerResponseException)
        {
            _logger.LogInformation(
                "Global exception caught server response exception with message {Exception}",
                exception.Message
            );
        }
        else
        {
            _logger.LogError(
                exception,
                "Global exception caught inner exception with message {Exception}",
                exception.Message
            );
        }
    }
}

using FluentLogger.Interfaces;
using Microsoft.AspNetCore.Diagnostics;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILog logger;
    public GlobalExceptionHandler(ILog logger)
    {
        this.logger = logger;
    }
    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("I'm in the TryHandleAsync");
        var exceptionMessage = exception.Message;
        logger.Error(exceptionMessage);
        // Return false to continue with the default behavior
        // - or - return true to signal that this exception is handled
        return ValueTask.FromResult(false);
    }
}

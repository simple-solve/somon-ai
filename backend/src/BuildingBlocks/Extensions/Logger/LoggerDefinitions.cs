namespace BuildingBlocks.Extensions.Logger;

/// <summary>
/// High-performance logging using source-generated LoggerMessage.
/// Based on Microsoft recommendations:
/// https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging.
/// </summary>
public static partial class LoggerDefinitions
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "[{Operation}] | START at {StartTime}")]
    public static partial void OperationStarted(
        this ILogger logger,
        string operation,
        DateTimeOffset startTime);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Information,
        Message = "[{Operation}] | COMPLETED at {CompletedTime} | Duration: {ElapsedMilliseconds} ms")]
    public static partial void OperationCompleted(
        this ILogger logger,
        string operation,
        DateTimeOffset completedTime,
        long elapsedMilliseconds);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Information,
        Message = "[{Operation}] | INFORMATION: {InformationMessage} ")]
    public static partial void OperationInfo(
        this ILogger logger,
        string operation,
        string informationMessage);

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Warning,
        Message = "[{Operation}] | WARNING: {WarningMessage}")]
    public static partial void OperationWarning(
        this ILogger logger,
        string operation,
        string warningMessage);

    [LoggerMessage(
        EventId = 1005,
        Level = LogLevel.Error,
        Message = "[{Operation}] | EXCEPTION: {ExceptionMessage}")]
    public static partial void OperationException(
        this ILogger logger,
        string operation,
        string exceptionMessage);

    [LoggerMessage(
        EventId = 1006,
        Level = LogLevel.Error,
        Message = "[{Operation}] | EXCEPTION")]
    public static partial void OperationException(
        this ILogger logger,
        Exception exception,
        string operation);

    [LoggerMessage(
        EventId = 1006,
        Level = LogLevel.Error,
        Message = "[{Operation}] | ERROR: {ErrorMessage}")]
    public static partial void OperationFail(
        this ILogger logger,
        string operation,
        string errorMessage);
}
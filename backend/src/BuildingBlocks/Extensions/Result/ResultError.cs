namespace BuildingBlocks.Extensions.Result;

/// <summary>
/// Represents a standardized error result used in the result pattern.
/// Encapsulates the HTTP-style error <see cref="Code"/>, a descriptive <see cref="Message"/>,
/// and an <see cref="ErrorType"/> indicating the nature of the error.
/// </summary>
public sealed record ResultError
{
    /// <summary>
    /// Gets the error code, typically aligned with HTTP status codes (e.g. 404, 500).
    /// </summary>
    public int? Code { get; init; }

    /// <summary>
    /// Gets a human-readable description of the error.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Gets the categorization of the error type.
    /// </summary>
    public ErrorType ErrorType { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultError"/> class with custom values.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The descriptive message for the error.</param>
    /// <param name="errorType">The error category.</param>
    private ResultError(int? code, string? message, ErrorType errorType)
    {
        Code = code;
        Message = message;
        ErrorType = errorType;
    }

    /// <summary>
    /// Creates a successful result with code 200 and optional message.
    /// </summary>
    /// <param name="message">The success message. Default is "Ok".</param>
    /// <returns>A <see cref="ResultError"/> instance representing success.</returns>
    public static ResultError None(string message = "Ok")
        => new(StatusCodes.Status200OK, message, ErrorType.None);

    /// <summary>
    /// Creates a result indicating that the requested data was not found (404).
    /// </summary>
    /// <param name="message">The error message. Default is "Data not found!".</param>
    /// <returns>A <see cref="ResultError"/> representing a not found error.</returns>
    public static ResultError NotFound(string? message = "Data not found!")
        => new(StatusCodes.Status404NotFound, message, ErrorType.NotFound);

    /// <summary>
    /// Creates a result indicating a bad request (400).
    /// </summary>
    /// <param name="message">The error message. Default is "Bad request!".</param>
    /// <returns>A <see cref="ResultError"/> representing a bad request.</returns>
    public static ResultError BadRequest(string? message = "Bad request!")
        => new(StatusCodes.Status400BadRequest, message, ErrorType.BadRequest);

    /// <summary>
    /// Creates a result indicating that the resource already exists (409).
    /// </summary>
    /// <param name="message">The error message. Default is "Already exist!".</param>
    /// <returns>A <see cref="ResultError"/> representing a resource conflict.</returns>
    public static ResultError AlreadyExist(string? message = "Already exist!")
        => new(StatusCodes.Status409Conflict, message, ErrorType.AlreadyExist);

    /// <summary>
    /// Creates a result indicating a conflict error (409).
    /// </summary>
    /// <param name="message">The error message. Default is "Conflict!".</param>
    /// <returns>A <see cref="ResultError"/> representing a conflict error.</returns>
    public static ResultError Conflict(string? message = "Conflict!")
        => new(StatusCodes.Status409Conflict, message, ErrorType.Conflict);

    /// <summary>
    /// Creates a result indicating an internal server error (500).
    /// </summary>
    /// <param name="message">The error message. Default is "Internal server error!".</param>
    /// <returns>A <see cref="ResultError"/> representing a server error.</returns>
    public static ResultError InternalServerError(string? message = "Internal server error!")
        => new(StatusCodes.Status500InternalServerError, message, ErrorType.InternalServerError);

    public static ResultError UnsupportedMediaType(string? message = "Unsupported Media Type!")
        => new(StatusCodes.Status415UnsupportedMediaType, message, ErrorType.UnsupportedMediaType);

    public static ResultError AccessDenied(string? message = "Access Denied!")
        => new(StatusCodes.Status403Forbidden, message, ErrorType.Forbidden);
}
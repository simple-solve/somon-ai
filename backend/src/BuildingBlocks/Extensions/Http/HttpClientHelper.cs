namespace BuildingBlocks.Extensions.Http;

/// <summary>
/// Provides helper methods for making HTTP requests to the Radix API.
/// These methods assist with sending POST requests and deserializing the response.
/// </summary>
public static class HttpClientHelper
{
    // Static logger for detailed logging. In production, consider using dependency injection.
    private static readonly ILogger Logger = LoggerFactory.Create(_ => { })
        .CreateLogger("HttpClientHelper");

    /// <summary>
    /// Sends an HTTP POST request with the specified request object and deserializes the response into the specified response type.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object to be sent in the body.</typeparam>
    /// <typeparam name="TResponse">The type of the response to be deserialized.</typeparam>
    /// <param name="httpClient">The HTTP client used to send the request.</param>
    /// <param name="url">The URL to which the POST request is sent.</param>
    /// <param name="request">The request object to be serialized and sent in the POST body.</param>
    /// <param name="token">The cancellation token to monitor task cancellation.</param>
    /// <returns>The deserialized response object of type TResponse, or null if the request was unsuccessful or an error occurred.</returns>
    public static async Task<Result<TResponse?>> PostAsync<TRequest, TResponse>(
        HttpClient httpClient,
        string url,
        TRequest request,
        CancellationToken token = default)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        Logger.OperationStarted(nameof(PostAsync), DateTimeOffset.UtcNow);

        try
        {
            string json = JsonConvert.SerializeObject(request);

            using StringContent content = new(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            using HttpResponseMessage response = await httpClient.PostAsync(url, content, token);


            if (response.IsSuccessStatusCode)
            {
                string stringRes = await response.Content.ReadAsStringAsync(token);
                TResponse? deserializedResponse = JsonConvert.DeserializeObject<TResponse>(stringRes);
                return Result<TResponse?>.Success(deserializedResponse);
            }


            string resContent = await response.Content.ReadAsStringAsync(token) + "\n" + response.ReasonPhrase;
            ResultError error = response.StatusCode switch
            {
                HttpStatusCode.OK => ResultError.None(),
                HttpStatusCode.Accepted => ResultError.None(),
                HttpStatusCode.BadRequest => ResultError.BadRequest(resContent),
                HttpStatusCode.NotFound => ResultError.NotFound(resContent),
                HttpStatusCode.Conflict => ResultError.Conflict(resContent),
                HttpStatusCode.UnsupportedMediaType => ResultError.UnsupportedMediaType(resContent),
                _ => ResultError.InternalServerError(resContent)
            };

            return Result<TResponse?>.Failure(error);
        }
        catch (Exception e)
        {
            Logger.OperationException(nameof(PostAsync), e.Message);
            return Result<TResponse?>.Failure(ResultError.InternalServerError(e.Message));
        }
        finally
        {
            stopwatch.Stop();
            Logger.OperationCompleted(nameof(PostAsync), DateTimeOffset.UtcNow, stopwatch.ElapsedMilliseconds);
        }
    }
}
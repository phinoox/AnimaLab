namespace Anima.Core.Dtos.Shared;

using System.Net;


/// <summary>
/// Base API response wrapper for all endpoints.
/// Provides type-safe, consistent responses across the application.
/// </summary>
public class ApiResponseDto<T> where T : class
{
    /// <summary>
    /// Indicates whether the request was successful.
    /// </summary>
    public bool Successful { get; set; } = true;

    /// <summary>
    /// The HTTP status code associated with this response.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    /// <summary>
    /// A descriptive message about the outcome (usually for errors).
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// A list of error messages if the operation failed.
    /// </summary>
    public List<string>? Errors { get; set; } = null!;

    /// <summary>
    /// The primary data payload returned upon a successful request.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Factory method to create a successful response containing the provided data.
    /// </summary>
    /// <param name="data">The data to be returned in the payload.</param>
    /// <returns>An ApiResponseDto representing success.</returns>
    public static ApiResponseDto<T> Success(T data) => new()
    {
        Successful = true,
        StatusCode = HttpStatusCode.OK,
        Message = null!,
        Errors = null!,
        Data = data
    };

    /// <summary>
    /// Factory method to create a 404 Not Found response.
    /// </summary>
    /// <param name="message">The error message describing why the resource was not found.</param>
    /// <returns>An ApiResponseDto representing a NotFound result.</returns>
    public static ApiResponseDto<T> NotFound(string message) => new()
    {
        Successful = false,
        StatusCode = HttpStatusCode.NotFound,
        Message = message,
        Errors = null!,
        Data = default!
    };

    /// <summary>
    /// Factory method to create a 400 Bad Request response.
    /// </summary>
    /// <param name="message">The error message describing the invalid request.</param>
    /// <returns>An ApiResponseDto representing a BadRequest result.</returns>
    public static ApiResponseDto<T> BadRequest(string message) => new()
    {
        Successful = false,
        StatusCode = HttpStatusCode.BadRequest,
        Message = message,
        Errors = null!,
        Data = default!
    };

    /// <summary>
    /// Factory method to create a 401 Unauthorized response.
    /// </summary>
    /// <param name="message">The error message describing the authentication failure.</param>
    /// <returns>An ApiResponseDto representing an Unauthorized result.</returns>
    public static ApiResponseDto<T> Unauthorized(string message) => new()
    {
        Successful = false,
        StatusCode = HttpStatusCode.Unauthorized,
        Message = message,
        Errors = null!,
        Data = default!
    };

    /// <summary>
    /// Factory method to create a 409 Conflict response (e.g., duplicate resource).
    /// </summary>
    /// <param name="message">The error message describing the conflict.</param>
    /// <returns>An ApiResponseDto representing a Conflict result.</returns>
    public static ApiResponseDto<T> Conflict(string message) => new()
    {
        Successful = false,
        StatusCode = HttpStatusCode.Conflict,
        Message = message,
        Errors = null!,
        Data = default!
    };

    /// <summary>
    /// Factory method to create a 403 Forbidden response.
    /// </summary>
    /// <param name="message">The error message describing the PermissionsEnum failure.</summary>
    /// <returns>An ApiResponseDto representing a Forbidden result.</returns>
    public static ApiResponseDto<T> Forbidden(string message) => new()
    {
        Successful = false,
        StatusCode = HttpStatusCode.Forbidden,
        Message = message,
        Errors = null!,
        Data = default!
    };

    /// <summary>
    /// Factory method to create a 500 Internal Server Error response.
    /// </summary>
    /// <param name="message">The error message describing the server-side failure.</param>
    /// <returns>An ApiResponseDto representing a ServerError result.</returns>
    public static ApiResponseDto<T> ServerError(string message) => new()
    {
        Successful = false,
        StatusCode = HttpStatusCode.InternalServerError,
        Message = message,
        Errors = null!,
        Data = default!
    };

    /// <summary>
    /// Factory method to create a 400 Bad Request response containing multiple validation errors.
    /// </summary>
    /// <param name="messages">The list of error messages.</param>
    /// <returns>An ApiResponseDto representing a BadRequest result with an error list.</returns>
    public static ApiResponseDto<T> WithErrors(List<string> messages) => new()
    {
        Successful = false,
        StatusCode = HttpStatusCode.BadRequest,
        Message = null!,
        Errors = messages ?? new List<string>(),
        Data = default!
    };
}
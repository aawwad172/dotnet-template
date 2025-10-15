namespace Dotnet.Template.Presentation.API.Models;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public int StatusCode { get; init; }
    public string? ErrorMessage { get; init; }
    public string? ErrorCode { get; init; }

    private ApiResponse(bool success, T? data, int statusCode, string? errorMessage, string? errorCode)
    {
        Success = success;
        Data = data;
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    // Factory methods
    public static ApiResponse<T> SuccessResponse(T data, int statusCode = 200)
        => new(true, data, statusCode, null, null);

    public static ApiResponse<T> ErrorResponse(string errorMessage, string errorCode, int statusCode)
        => new(false, default, statusCode, errorMessage, errorCode);
}

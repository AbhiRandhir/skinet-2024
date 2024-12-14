using System;

namespace API.Errors;

public class ApiErrorResponse(int statusCode, string meesage, string? details)
{
    public int StatusCode { get; set; } = statusCode;
    public string Message { get; set; } = meesage;
    public string? Details { get; set; } = details;
}

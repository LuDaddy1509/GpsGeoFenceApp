using Microsoft.AspNetCore.Mvc;

namespace MapApi.Common;

public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult Error(int statusCode, string code, string message, Dictionary<string, string[]>? details = null)
    {
        var response = new ApiErrorResponse
        {
            Code = code,
            Message = message,
            Details = details,
            TraceId = HttpContext.TraceIdentifier
        };

        return StatusCode(statusCode, response);
    }

    protected ActionResult ValidationError(Dictionary<string, string[]> details) =>
        Error(StatusCodes.Status400BadRequest, "validation_error", "Validation failed.", details);

    protected ActionResult NotFoundError(string message) =>
        Error(StatusCodes.Status404NotFound, "not_found", message);

    protected ActionResult ConflictError(string message) =>
        Error(StatusCodes.Status409Conflict, "conflict", message);

    protected ActionResult UnauthorizedError(string message = "Unauthorized.") =>
        Error(StatusCodes.Status401Unauthorized, "unauthorized", message);
}

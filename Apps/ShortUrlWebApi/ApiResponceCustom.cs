using Microsoft.AspNetCore.Mvc;
using ShortUrlAppWebAPI.DTO;

namespace ShortUrlAppWebAPI;
public static class ApiResponseCustom
{
    public static IActionResult CreateBadRequestResponse<T>(this ControllerBase controller, ApiStatusCode statusCode)
    {
        return controller.BadRequest(ApiResponseDTO.CreateResponse<T>(statusCode));
    }
    public static IActionResult CreateNotFoundResponse<T>(this ControllerBase controller, ApiStatusCode statusCode)
    {
        return controller.NotFound(ApiResponseDTO.CreateResponse<T>(statusCode));
    }
    public static IActionResult CreateSuccessResponse<T>(this ControllerBase controller, ApiStatusCode statusCode, T data = default)
    {
        return controller.Ok(ApiResponseDTO.CreateResponse(statusCode, data));
    }

    public static IActionResult CreateErrorResponse<T>(this ControllerBase controller, ApiStatusCode statusCode)
    {
        return controller.StatusCode(500, ApiResponseDTO.CreateResponse<T>(statusCode));
    }
}
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Api.Common;

/// <summary>
/// Base controller for all stateless v1 mobile JSON endpoints.
/// Centralizes routing, antiforgery exemption, and consistent error/success responses.
/// </summary>
[ApiController]
[IgnoreAntiforgeryToken]
[Route(ApiRoutes.V1Prefix)]
public abstract class MobileApiControllerBase : ControllerBase
{
    protected ActionResult Fail(string error)
        => BadRequest(new ApiErrorResponse(error));

    protected ActionResult NotFoundError(string error)
        => NotFound(new ApiErrorResponse(error));

    protected ActionResult ConflictError(string error)
        => Conflict(new ApiErrorResponse(error));

    protected ActionResult UnauthorizedError(string error)
        => Unauthorized(new ApiErrorResponse(error));

    protected ActionResult OkMessage(string message)
        => Ok(new ApiMessageResponse(message));
}

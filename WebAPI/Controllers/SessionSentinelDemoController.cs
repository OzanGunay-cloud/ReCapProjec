using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SessionSentinel.Application.Abstractions;
using SessionSentinel.WebApi;

namespace WebAPI.Controllers;

[Route("api/session-sentinel-demo")]
[ApiController]
public sealed class SessionSentinelDemoController : ControllerBase
{
    private readonly IUserIdentityAccessor _userIdentityAccessor;

    public SessionSentinelDemoController(IUserIdentityAccessor userIdentityAccessor)
    {
        _userIdentityAccessor = userIdentityAccessor;
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            UserId = _userIdentityAccessor.GetUserId(User),
            SessionId = User.FindFirst("jti")?.Value,
            ServerSeenIp = ClientIpResolver.Resolve(HttpContext),
            ForwardedForHeader = Request.Headers[ClientIpResolver.ForwardedForHeaderName].ToString(),
            FingerprintHeader = Request.Headers["X-Sentinel-Fingerprint"].ToString(),
            Message = "Session Sentinel protected endpoint reached."
        });
    }
}

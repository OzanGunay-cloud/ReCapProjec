using Business.Abstract;
using Core.Entities.Concrete;
using Core.Entities.DTOs;
using Core.Utilities.Security.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SessionSentinel.Application.Abstractions;
using SessionSentinel.Application.Contracts;
using SessionSentinel.WebApi;
using SessionSentinel.WebApi.Http;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Options;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ISessionRegistrationService _sessionRegistrationService;
        private readonly ISessionRevocationService _sessionRevocationService;
        private readonly ITokenHasher _tokenHasher;
        private readonly IUserIdentityAccessor _userIdentityAccessor;
        private readonly ILoginChallengeService _loginChallengeService;
        private readonly LoginChallengeOptions _loginChallengeOptions;

        public AuthController(
            IAuthService authService,
            IUserService userService,
            ISessionRegistrationService sessionRegistrationService,
            ISessionRevocationService sessionRevocationService,
            ITokenHasher tokenHasher,
            IUserIdentityAccessor userIdentityAccessor,
            ILoginChallengeService loginChallengeService,
            IOptions<LoginChallengeOptions> loginChallengeOptions)
        {
            _authService = authService;
            _userService = userService;
            _sessionRegistrationService = sessionRegistrationService;
            _sessionRevocationService = sessionRevocationService;
            _tokenHasher = tokenHasher;
            _userIdentityAccessor = userIdentityAccessor;
            _loginChallengeService = loginChallengeService;
            _loginChallengeOptions = loginChallengeOptions.Value;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserForLoginDto userForLoginDto, CancellationToken cancellationToken)
        {
            var userToLogin = await _authService.Login(userForLoginDto);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin.Message);
            }

            var challenge = await CreateLoginChallengeIfRequiredAsync(userToLogin.Data, cancellationToken);
            if (challenge is not null)
            {
                return Ok(new
                {
                    ChallengeRequired = true,
                    challenge.ChallengeId,
                    challenge.ExpiresAtUtc,
                    challenge.DevelopmentCode,
                    challenge.Message
                });
            }

            var result = await _authService.CreateAccessToken(userToLogin.Data);
            if (result.Success)
            {
                await RegisterSessionIfFingerprintProvidedAsync(userToLogin.Data.Id.ToString(), result.Data, cancellationToken);
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserForRegisterDto userForRegisterDto, CancellationToken cancellationToken)
        {
            var userExists = await _authService.UserExists(userForRegisterDto.Email);
            if (!userExists.Success)
            {
                return BadRequest(userExists.Message);
            }

            var registerResult = await _authService.Register(userForRegisterDto, userForRegisterDto.Password);
            var result = await _authService.CreateAccessToken(registerResult.Data);
            if (result.Success)
            {
                await RegisterSessionIfFingerprintProvidedAsync(registerResult.Data.Id.ToString(), result.Data, cancellationToken);
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }

        [HttpPost("verify-login-challenge")]
        public async Task<ActionResult> VerifyLoginChallenge(
            VerifyLoginChallengeRequest request,
            CancellationToken cancellationToken)
        {
            var verificationResult = await _loginChallengeService.VerifyAsync(
                request.ChallengeId,
                request.Code,
                cancellationToken);

            if (!verificationResult.IsSuccess || verificationResult.Challenge is null)
            {
                return BadRequest(new { Message = verificationResult.ErrorMessage });
            }

            var user = await _userService.GetByMail(verificationResult.Challenge.Email);
            if (user is null)
            {
                return BadRequest(new { Message = "User no longer exists." });
            }

            var tokenResult = await _authService.CreateAccessToken(user);
            if (!tokenResult.Success)
            {
                return BadRequest(tokenResult.Message);
            }

            await RegisterVerifiedSessionAsync(
                verificationResult.Challenge,
                tokenResult.Data,
                cancellationToken);

            return Ok(tokenResult.Data);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout(RevokeHttpSessionRequest request, CancellationToken cancellationToken)
        {
            var userId = _userIdentityAccessor.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var authorization = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authorization) ||
                !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized();
            }

            var rawToken = authorization["Bearer ".Length..].Trim();
            var tokenHash = _tokenHasher.HashToken(rawToken);

            await _sessionRevocationService.RevokeAsync(
                new RevokeSessionRequest(
                    User.FindFirst("jti")?.Value ?? tokenHash,
                    userId,
                    tokenHash,
                    null,
                    string.IsNullOrWhiteSpace(request.Reason) ? "User logout" : request.Reason,
                    DateTime.UtcNow),
                cancellationToken);

            return Ok(new { Revoked = true, SessionId = User.FindFirst("jti")?.Value ?? tokenHash });
        }

        private async Task RegisterSessionIfFingerprintProvidedAsync(
            string userId,
            AccessToken accessToken,
            CancellationToken cancellationToken)
        {
            var fingerprintHash = Request.Headers[SessionSentinelMiddleware.FingerprintHeaderName].ToString();
            if (string.IsNullOrWhiteSpace(fingerprintHash))
            {
                return;
            }

            // Login sirasinda fingerprint geldiyse aktif oturum icin baslangic snapshot'i acilir.
            await _sessionRegistrationService.RegisterAsync(
                new RegisterSessionRequest(
                    accessToken.SessionId,
                    userId,
                    ClientIpResolver.Resolve(HttpContext),
                    fingerprintHash,
                    Request.Headers["User-Agent"].ToString(),
                    Request.Headers["Accept-Language"].ToString(),
                    null,
                    DateTime.UtcNow),
                cancellationToken);
        }

        private async Task<LoginChallengeInitiationResult?> CreateLoginChallengeIfRequiredAsync(
            User user,
            CancellationToken cancellationToken)
        {
            if (!_loginChallengeOptions.Enabled)
            {
                return null;
            }

            var fingerprintHash = Request.Headers[SessionSentinelMiddleware.FingerprintHeaderName].ToString();
            if (string.IsNullOrWhiteSpace(fingerprintHash))
            {
                return null;
            }

            return await _loginChallengeService.CreateIfRequiredAsync(
                user,
                ClientIpResolver.Resolve(HttpContext),
                fingerprintHash,
                Request.Headers["User-Agent"].ToString(),
                Request.Headers["Accept-Language"].ToString(),
                cancellationToken);
        }

        private async Task RegisterVerifiedSessionAsync(
            PendingLoginChallenge challenge,
            AccessToken accessToken,
            CancellationToken cancellationToken)
        {
            // Challenge dogrulandiktan sonra token icindeki session id ile oturum acilir.
            await _sessionRegistrationService.RegisterAsync(
                new RegisterSessionRequest(
                    accessToken.SessionId,
                    challenge.UserId,
                    challenge.IpAddress,
                    challenge.FingerprintHash,
                    challenge.UserAgent,
                    challenge.Language,
                    null,
                    DateTime.UtcNow),
                cancellationToken);
        }
    }
}

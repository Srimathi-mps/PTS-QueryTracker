using Microsoft.AspNetCore.Mvc;
using QueryTracker.Api.Models;
using QueryTracker.Api.Services;

namespace QueryTracker.Api.Controllers
{
    /// <summary>
    /// Handles username/password authentication.
    /// Route: POST /api/auth/login
    ///
    /// This controller was missing from the original migration, causing Login.jsx
    /// to receive HTTP 404 on every login attempt.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ── POST /api/auth/login ──────────────────────────────────────────────
        /// <summary>
        /// Validates credentials and returns the authenticated userName.
        /// The frontend (Login.jsx) stores the returned userName in sessionStorage.
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName))
                return BadRequest(new { message = "userName is required" });

            var result = _authService.ValidateUser(request.UserName, request.Password ?? string.Empty);

            if (result == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(new LoginResponse { UserName = result });
        }
    }
}

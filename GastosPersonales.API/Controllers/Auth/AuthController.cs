using Microsoft.AspNetCore.Mvc;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.DTOs.User;
using Microsoft.AspNetCore.Authorization;

namespace GastosPersonales.API.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) { _auth = auth; }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var res = await _auth.Register(req);
            return Ok(res);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var res = await _auth.Login(req);
            return Ok(res);
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var correo = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            return Ok(new { Id = sub, Correo = correo });
        }
    }
}

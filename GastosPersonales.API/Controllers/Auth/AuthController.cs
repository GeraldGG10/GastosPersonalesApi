using Microsoft.AspNetCore.Mvc;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GastosPersonales.API.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        
        public AuthController(IAuthService auth) 
        { 
            _auth = auth; 
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var res = await _auth.Register(req);
            if (string.IsNullOrEmpty(res.Token))
            {
                return BadRequest(new { Message = "El usuario ya existe o datos inválidos" });
            }
            return Ok(res);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var res = await _auth.Login(req);
            if (string.IsNullOrEmpty(res.Token))
            {
                return Unauthorized(new { Message = "Credenciales inválidas" });
            }
            return Ok(res);
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var correo = User.FindFirst(ClaimTypes.Email)?.Value;
            var sub = User.FindFirst("sub")?.Value;
            return Ok(new { Id = sub, Correo = correo });
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            // Obtener userId del token JWT
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { Message = "Usuario no autenticado" });
            }

            var success = await _auth.UpdateProfile(userId, dto);
            if (!success)
            {
                return NotFound(new { Message = "Usuario no encontrado" });
            }

            return Ok(new { Message = "Perfil actualizado correctamente" });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            // Obtener userId del token JWT
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { Message = "Usuario no autenticado" });
            }

            var success = await _auth.ChangePassword(userId, dto);
            if (!success)
            {
                return BadRequest(new { Message = "Contraseña actual incorrecta" });
            }

            return Ok(new { Message = "Contraseña cambiada correctamente" });
        }
    }
}

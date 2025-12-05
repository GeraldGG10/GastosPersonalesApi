# modulo-A.ps1
param()
$ErrorActionPreference = "Stop"
cd (Get-Location)

Write-Host "Módulo A: Autenticación (hashing, registro, login, perfil)..."

# Paquetes necesarios
dotnet add .\GastosPersonales.Infrastructure\GastosPersonales.Infrastructure.csproj package BCrypt.Net-Next --version 4.0.2
dotnet add .\GastosPersonales.API\GastosPersonales.API.csproj package FluentValidation.AspNetCore --version 11.5.2
dotnet add .\GastosPersonales.API\GastosPersonales.API.csproj package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1

# Auth DTOs (Application)
New-Item -ItemType Directory -Path ".\GastosPersonales.Application\DTOs\User" -Force | Out-Null
@'
namespace GastosPersonales.Application.DTOs.User
{
    public record RegisterRequest(string Nombre, string Correo, string Password);
    public record LoginRequest(string Correo, string Password);
    public record UserProfileResponse(int Id, string Nombre, string Correo);
    public record ChangePasswordRequest(string OldPassword, string NewPassword);
    public record ChangeNameRequest(string Nombre);
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.Application\DTOs\User\AuthDtos.cs

# AuthService (Application implementation already exists, we update Infrastructure/GeneradorJwt used)
# Update AuthService implementation (overwrite)
@'
using GastosPersonales.Application.DTOs.User;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Interfaces;
using System.Threading.Tasks;

namespace GastosPersonales.Application.Services.Implementations
{
    public class AuthService : Services.Interfaces.IAuthService
    {
        private readonly IUsuarioRepositorio _usuarios;
        private readonly IGeneradorJwt _jwt;

        public AuthService(IUsuarioRepositorio usuarios, IGeneradorJwt jwt)
        {
            _usuarios = usuarios;
            _jwt = jwt;
        }

        public async Task<AuthResponse> Register(RegisterRequest request)
        {
            var existing = await _usuarios.ObtenerPorCorreoAsync(request.Correo);
            if (existing != null) throw new System.Exception("Correo ya registrado");

            var user = new Usuario { Nombre = request.Nombre, Correo = request.Correo };
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            await _usuarios.AgregarAsync(user);
            var token = _jwt.GenerarToken(user.Id, user.Correo);
            return new AuthResponse { Token = token, Correo = user.Correo };
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            var user = await _usuarios.ObtenerPorCorreoAsync(request.Correo) ?? throw new System.Exception("Usuario no encontrado");
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) throw new System.Exception("Credenciales inválidas");
            var token = _jwt.GenerarToken(user.Id, user.Correo);
            return new AuthResponse { Token = token, Correo = user.Correo };
        }
    }
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.Application\Services\Implementations\AuthService.cs

# Exponer endpoints de auth en API
New-Item -ItemType Directory -Path ".\GastosPersonales.API\Controllers\Auth" -Force | Out-Null
@'
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
'@ | Out-File -Encoding utf8 .\GastosPersonales.API\Controllers\Auth\AuthController.cs

# Register FluentValidation sample (Validator for RegisterRequest)
New-Item -ItemType Directory -Path ".\GastosPersonales.Application\Validators" -Force | Out-Null
@'
using FluentValidation;
using GastosPersonales.Application.DTOs.User;

namespace GastosPersonales.Application.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("Nombre requerido");
            RuleFor(x => x.Correo).NotEmpty().EmailAddress().WithMessage("Correo inválido");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Contraseña mínima 6 caracteres");
        }
    }
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.Application\Validators\RegisterRequestValidator.cs

# Update Program.cs to add FluentValidation, AutoMapper (append minimal config)
$prog = Get-Content .\GastosPersonales.API\Program.cs
if (-not ($prog -match "AddFluentValidation")) {
    $extra = "`n// FluentValidation & AutoMapper`nbuilder.Services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining(typeof(GastosPersonales.Application.Validators.RegisterRequestValidator)));`nbuilder.Services.AddAutoMapper(typeof(Program));`n"
    $prog = $prog -replace "builder.Services.AddControllers\(\);", $extra
    $prog | Set-Content .\GastosPersonales.API\Program.cs
}

dotnet restore
dotnet build
Write-Host "Módulo A completado."

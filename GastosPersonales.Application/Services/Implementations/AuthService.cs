using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.DTOs.Auth;
using GastosPersonales.Domain.Interfaces;
using GastosPersonales.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GastosPersonales.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public AuthService(IConfiguration configuration, IUsuarioRepositorio usuarioRepositorio)
        {
            _configuration = configuration;
            _usuarioRepositorio = usuarioRepositorio;
        }

        public async Task<AuthResponse> Register(RegisterRequest request)
        {
            // Verificar si ya existe
            var existingUser = await _usuarioRepositorio.ObtenerPorCorreoAsync(request.Email);
            if (existingUser != null)
            {
                return new AuthResponse
                {
                    Token = string.Empty,
                    Email = string.Empty,
                    FullName = string.Empty
                };
            }

            // Crear usuario con hash
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new Usuario
            {
                Nombre = request.FullName,
                Correo = request.Email,
                PasswordHash = passwordHash
            };

            await _usuarioRepositorio.AgregarAsync(user);

            // Generar token JWT real
            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                Email = user.Correo,
                FullName = user.Nombre
            };
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            var user = await _usuarioRepositorio.ObtenerPorCorreoAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResponse
                {
                    Token = string.Empty,
                    Email = string.Empty,
                    FullName = string.Empty
                };
            }

            // Generar token JWT real
            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                Email = user.Correo,
                FullName = user.Nombre
            };
        }

        public async Task<bool> UpdateProfile(int userId, UpdateProfileDTO dto)
        {
            var user = await _usuarioRepositorio.ObtenerPorIdAsync(userId);
            if (user == null) return false;

            user.Nombre = dto.Nombre;
            await _usuarioRepositorio.ActualizarAsync(user);
            return true;
        }

        public async Task<bool> ChangePassword(int userId, ChangePasswordDTO dto)
        {
            var user = await _usuarioRepositorio.ObtenerPorIdAsync(userId);
            if (user == null) return false;

            // Verificar contraseña actual
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                return false;
            }

            // Actualizar contraseña
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _usuarioRepositorio.ActualizarAsync(user);
            return true;
        }

        public async Task<UserProfileDTO?> GetProfile(int userId)
        {
            var user = await _usuarioRepositorio.ObtenerPorIdAsync(userId);
            if (user == null) return null;

            return new UserProfileDTO
            {
                Id = user.Id,
                Email = user.Correo,
                FullName = user.Nombre
            };
        }

        // ✅ MÉTODO NUEVO: Generar token JWT real
        private string GenerateJwtToken(Usuario user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "tu-clave-secreta-super-segura-de-al-menos-32-caracteres-para-jwt")
            );
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Correo),
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim("sub", user.Id.ToString()) // Para compatibilidad
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "GastosPersonalesAPI",
                audience: _configuration["Jwt:Audience"] ?? "GastosPersonalesFrontend",
                claims: claims,
                expires: DateTime.Now.AddDays(7), // Token válido por 7 días
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
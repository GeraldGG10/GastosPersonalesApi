using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.DTOs.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GastosPersonales.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        // Simulación de base de datos en memoria (temporal)
        private static List<UserData> _users = new List<UserData>();

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<AuthResponse> Register(RegisterRequest request)
        {
            // Verificar si ya existe
            if (_users.Any(u => u.Email == request.Email))
            {
                return Task.FromResult(new AuthResponse
                {
                    Token = string.Empty,
                    Email = string.Empty,
                    FullName = string.Empty
                });
            }

            // Crear usuario con hash
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new UserData
            {
                Id = _users.Count + 1,
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = passwordHash
            };

            _users.Add(user);

            // Generar token JWT real
            var token = GenerateJwtToken(user);

            var response = new AuthResponse
            {
                Token = token,
                Email = user.Email,
                FullName = user.FullName
            };

            return Task.FromResult(response);
        }

        public Task<AuthResponse> Login(LoginRequest request)
        {
            var user = _users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Task.FromResult(new AuthResponse
                {
                    Token = string.Empty,
                    Email = string.Empty,
                    FullName = string.Empty
                });
            }

            // Generar token JWT real
            var token = GenerateJwtToken(user);

            var response = new AuthResponse
            {
                Token = token,
                Email = user.Email,
                FullName = user.FullName
            };

            return Task.FromResult(response);
        }

        public Task<bool> UpdateProfile(int userId, UpdateProfileDTO dto)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return Task.FromResult(false);

            user.FullName = dto.Nombre;
            return Task.FromResult(true);
        }

        public Task<bool> ChangePassword(int userId, ChangePasswordDTO dto)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return Task.FromResult(false);

            // Verificar contraseña actual
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                return Task.FromResult(false);
            }

            // Actualizar contraseña
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            return Task.FromResult(true);
        }

        // ✅ MÉTODO NUEVO: Generar token JWT real
        private string GenerateJwtToken(UserData user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "tu-clave-secreta-super-segura-de-al-menos-32-caracteres")
            );
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
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

        // Clase interna para almacenar usuarios temporalmente
        private class UserData
        {
            public int Id { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;
        }
    }
}
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.DTOs.Auth;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace GastosPersonales.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        // Simulación de base de datos en memoria (temporal)
        private static List<UserData> _users = new List<UserData>();

        public Task<AuthResponse> Register(RegisterRequest request)
        {
            // Verificar si ya existe
            if (_users.Any(u => u.Correo == request.Correo))
            {
                return Task.FromResult(new AuthResponse
                {
                    Token = string.Empty,
                    Correo = string.Empty
                });
            }

            // Crear usuario con hash
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new UserData
            {
                Id = _users.Count + 1,
                Nombre = request.Nombre,
                Correo = request.Correo,
                PasswordHash = passwordHash
            };

            _users.Add(user);

            var response = new AuthResponse
            {
                Token = "dummy-token-" + user.Id,
                Correo = request.Correo
            };
            return Task.FromResult(response);
        }

        public Task<AuthResponse> Login(LoginRequest request)
        {
            var user = _users.FirstOrDefault(u => u.Correo == request.Correo);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Task.FromResult(new AuthResponse
                {
                    Token = string.Empty,
                    Correo = string.Empty
                });
            }

            var response = new AuthResponse
            {
                Token = "dummy-token-" + user.Id,
                Correo = request.Correo
            };
            return Task.FromResult(response);
        }

        public Task<bool> UpdateProfile(int userId, UpdateProfileDTO dto)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return Task.FromResult(false);

            user.Nombre = dto.Nombre;
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

        // Clase interna para almacenar usuarios temporalmente
        private class UserData
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Correo { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;
        }
    }
}

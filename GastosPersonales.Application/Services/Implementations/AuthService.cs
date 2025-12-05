using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.DTOs.Auth;
using System.Threading.Tasks;

namespace GastosPersonales.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        public Task<AuthResponse> Register(RegisterRequest request)
        {
            // Lógica temporal de registro
            var response = new AuthResponse
            {
                Token = "dummy-token",
                Correo = request.Correo
            };
            return Task.FromResult(response);
        }

        public Task<AuthResponse> Login(LoginRequest request)
        {
            // Lógica temporal de login
            var response = new AuthResponse
            {
                Token = "dummy-token",
                Correo = request.Correo
            };
            return Task.FromResult(response);
        }
    }
}

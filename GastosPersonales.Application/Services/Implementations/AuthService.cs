using GastosPersonales.Application.Models;

namespace GastosPersonales.Application.Services.Implementations
{
    public interface IAuthService
    {
        AuthResponse Register(RegisterRequest request);
        AuthResponse Login(LoginRequest request);
    }

    public class AuthService : IAuthService
    {
        public AuthResponse Register(RegisterRequest request)
        {
            // Lógica temporal de registro
            return new AuthResponse
            {
                Token = "dummy-token",
                Message = "Usuario registrado correctamente"
            };
        }

        public AuthResponse Login(LoginRequest request)
        {
            // Lógica temporal de login
            return new AuthResponse
            {
                Token = "dummy-token",
                Message = "Login exitoso"
            };
        }
    }
}


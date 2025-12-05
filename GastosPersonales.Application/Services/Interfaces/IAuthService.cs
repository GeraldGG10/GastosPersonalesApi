using GastosPersonales.Application.DTOs.Auth;

namespace GastosPersonales.Application.Services.Interfaces
{
    public interface IAuthService
    {
        System.Threading.Tasks.Task<AuthResponse> Register(RegisterRequest request);
        System.Threading.Tasks.Task<AuthResponse> Login(LoginRequest request);
    }
}

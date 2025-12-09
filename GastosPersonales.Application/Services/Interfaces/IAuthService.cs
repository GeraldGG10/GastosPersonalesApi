using GastosPersonales.Application.DTOs.Auth;

namespace GastosPersonales.Application.Services.Interfaces
{
    // Interfaz para el servicio de autenticación y gestión de usuarios
    public interface IAuthService
    {
        System.Threading.Tasks.Task<AuthResponse> Register(RegisterRequest request);
        System.Threading.Tasks.Task<AuthResponse> Login(LoginRequest request);
        System.Threading.Tasks.Task<bool> UpdateProfile(int userId, UpdateProfileDTO dto);
        System.Threading.Tasks.Task<bool> ChangePassword(int userId, ChangePasswordDTO dto);
        System.Threading.Tasks.Task<UserProfileDTO?> GetProfile(int userId);
    }
}

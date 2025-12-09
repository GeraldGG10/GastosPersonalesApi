namespace GastosPersonales.Application.DTOs.Auth
{
    //Objetos de Transferencia de Datos (DTO) para autenticación y gestión de usuarios

    // DTO para el Login
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // DTO para el Registro
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    // DTO para la respuesta de autenticación
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    // DTO para actualizar el perfil
    public class UpdateProfileDTO
    {
        public string Nombre { get; set; } = string.Empty;
    }

    // DTO para cambiar la contraseña
    public class ChangePasswordDTO
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UserProfileDTO
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}

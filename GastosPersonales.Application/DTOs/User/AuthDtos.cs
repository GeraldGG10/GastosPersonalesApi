namespace GastosPersonales.Application.DTOs.User
{
    // DTOs para la autenticación y gestión de usuarios
    public record RegisterRequest(string Nombre, string Correo, string Password);
    public record LoginRequest(string Correo, string Password);
    public record UserProfileResponse(int Id, string Nombre, string Correo);
    public record ChangePasswordRequest(string OldPassword, string NewPassword);
    public record ChangeNameRequest(string Nombre);
}

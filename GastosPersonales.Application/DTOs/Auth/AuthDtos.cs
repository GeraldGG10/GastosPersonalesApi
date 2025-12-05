namespace GastosPersonales.Application.DTOs.Auth
{
    public class RegisterRequest { public string Nombre { get; set; } = ""; public string Correo { get; set; } = ""; public string Password { get; set; } = ""; }
    public class LoginRequest { public string Correo { get; set; } = ""; public string Password { get; set; } = ""; }
    public class AuthResponse { public string Token { get; set; } = ""; public string Correo { get; set; } = ""; }
}

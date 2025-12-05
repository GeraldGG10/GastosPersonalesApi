using GastosPersonales.Application.Models;

namespace GastosPersonales.Application.Models
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        // Agrega más propiedades si tu aplicación las necesita
    }
}

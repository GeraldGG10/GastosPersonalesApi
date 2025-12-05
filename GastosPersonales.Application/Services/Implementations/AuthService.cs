using GastosPersonales.Application.DTOs.Auth;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Interfaces;

namespace GastosPersonales.Application.Services.Implementations
{
    public class AuthService : Services.Interfaces.IAuthService
    {
        private readonly IUsuarioRepositorio _usuarios;
        private readonly IGeneradorJwt _jwt;

        public AuthService(IUsuarioRepositorio usuarios, IGeneradorJwt jwt)
        {
            _usuarios = usuarios;
            _jwt = jwt;
        }

        public async System.Threading.Tasks.Task<AuthResponse> Register(RegisterRequest request)
        {
            var user = new Usuario { Nombre = request.Nombre, Correo = request.Correo, PasswordHash = request.Password };
            await _usuarios.AgregarAsync(user);
            var token = _jwt.GenerarToken(user.Id, user.Correo);
            return new AuthResponse { Token = token, Correo = user.Correo };
        }

        public async System.Threading.Tasks.Task<AuthResponse> Login(LoginRequest request)
        {
            var user = await _usuarios.ObtenerPorCorreoAsync(request.Correo) ?? throw new System.Exception("Usuario no encontrado");
            var token = _jwt.GenerarToken(user.Id, user.Correo);
            return new AuthResponse { Token = token, Correo = user.Correo };
        }
    }
}

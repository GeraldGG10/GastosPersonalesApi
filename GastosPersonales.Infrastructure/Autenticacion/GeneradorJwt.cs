using GastosPersonales.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GastosPersonales.Infrastructure.Autenticacion
{
    public class GeneradorJwt : IGeneradorJwt
    {
        private readonly IConfiguration _config;
        public GeneradorJwt(IConfiguration config) { _config = config; }

        public string GenerarToken(int usuarioId, string correo)
        {
            var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Clave"] ?? "clave-secreta-default"));
            var cred = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);
            var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, usuarioId.ToString()), new Claim(JwtRegisteredClaimNames.Email, correo) };
            var token = new JwtSecurityToken(issuer: _config["Jwt:Emisor"], audience: _config["Jwt:Audiencia"], claims: claims, expires: System.DateTime.UtcNow.AddHours(2), signingCredentials: cred);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

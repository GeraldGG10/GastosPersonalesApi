using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Interfaces;
using GastosPersonales.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace GastosPersonales.Infrastructure.Repositorios
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly AplicacionDbContext _contexto;
        public UsuarioRepositorio(AplicacionDbContext contexto) { _contexto = contexto; }

        public async System.Threading.Tasks.Task AgregarAsync(Usuario usuario)
        {
            _contexto.Usuarios.Add(usuario);
            await _contexto.SaveChangesAsync();
        }

        public System.Threading.Tasks.Task<Usuario?> ObtenerPorCorreoAsync(string correo) => _contexto.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
        public System.Threading.Tasks.Task<Usuario?> ObtenerPorIdAsync(int id) => _contexto.Usuarios.FindAsync(id).AsTask();

        public async System.Threading.Tasks.Task ActualizarAsync(Usuario usuario)
        {
            _contexto.Usuarios.Update(usuario);
            await _contexto.SaveChangesAsync();
        }
    }
}

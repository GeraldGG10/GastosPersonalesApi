using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Interfaces;
using GastosPersonales.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace GastosPersonales.Infrastructure.Repositorios
{
    public class CategoriaRepositorio : ICategoriaRepositorio
    {
        private readonly AplicacionDbContext _contexto;
        public CategoriaRepositorio(AplicacionDbContext contexto) { _contexto = contexto; }

        public async System.Threading.Tasks.Task AgregarAsync(Categoria categoria)
        {
            _contexto.Categorias.Add(categoria);
            await _contexto.SaveChangesAsync();
        }

        public System.Threading.Tasks.Task<System.Collections.Generic.List<Categoria>> ObtenerPorUsuarioIdAsync(int usuarioId)
            => _contexto.Categorias.Where(c => c.UsuarioId == usuarioId).ToListAsync();
    }
}

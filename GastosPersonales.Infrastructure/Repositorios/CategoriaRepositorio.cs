using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Interfaces;
using GastosPersonales.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace GastosPersonales.Infrastructure.Repositorios
{
    // Implementación del repositorio para la entidad Categoria
    public class CategoriaRepositorio : ICategoriaRepositorio
    {
        private readonly AplicacionDbContext _contexto;
        public CategoriaRepositorio(AplicacionDbContext contexto) { _contexto = contexto; }

        // Agrega una nueva categoría a la base de datos
        public async System.Threading.Tasks.Task AgregarAsync(Categoria categoria)
        {
            _contexto.Categorias.Add(categoria);
            await _contexto.SaveChangesAsync();
        }

        // Obtiene todas las categorías asociadas a un usuario específico
        public System.Threading.Tasks.Task<System.Collections.Generic.List<Categoria>> ObtenerPorUsuarioIdAsync(int usuarioId)
            => _contexto.Categorias.Where(c => c.UsuarioId == usuarioId).ToListAsync();

        public System.Threading.Tasks.Task<Categoria?> ObtenerPorIdAsync(int id) => _contexto.Categorias.FindAsync(id).AsTask();

        // PAra actualizar una categoría existente en la base de datos
        public async System.Threading.Tasks.Task ActualizarAsync(Categoria categoria)
        {
            _contexto.Categorias.Update(categoria);
            await _contexto.SaveChangesAsync();
        }

        // Eliminacion una categoría de la base de datos
        public async System.Threading.Tasks.Task EliminarAsync(Categoria categoria)
        {
            _contexto.Categorias.Remove(categoria);
            await _contexto.SaveChangesAsync();
        }
    }
}

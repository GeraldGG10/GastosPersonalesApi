using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Interfaces;
using GastosPersonales.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace GastosPersonales.Infrastructure.Repositorios
{
    // Repositorio para la gestión de gastos
    public class GastoRepositorio : IGastoRepositorio
    {
        private readonly AplicacionDbContext _contexto;
        public GastoRepositorio(AplicacionDbContext contexto) { _contexto = contexto; }

        // Agrega un nuevo gasto a la base de datos
        public async System.Threading.Tasks.Task AgregarAsync(Gasto gasto)
        {
            _contexto.Gastos.Add(gasto);
            await _contexto.SaveChangesAsync();
        }

        public System.Threading.Tasks.Task<System.Collections.Generic.List<Gasto>> ObtenerPorUsuarioIdAsync(int usuarioId)
            => _contexto.Gastos.Where(g => g.UsuarioId == usuarioId).ToListAsync();
    }
}

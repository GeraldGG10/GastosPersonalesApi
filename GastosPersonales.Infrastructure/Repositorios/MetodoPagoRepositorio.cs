using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Interfaces;
using GastosPersonales.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace GastosPersonales.Infrastructure.Repositorios
{
    public class MetodoPagoRepositorio : IMetodoPagoRepositorio
    {
        private readonly AplicacionDbContext _contexto;
        public MetodoPagoRepositorio(AplicacionDbContext contexto) { _contexto = contexto; }

        public async System.Threading.Tasks.Task AgregarAsync(MetodoPago metodo)
        {
            _contexto.MetodosPago.Add(metodo);
            await _contexto.SaveChangesAsync();
        }

        public System.Threading.Tasks.Task<System.Collections.Generic.List<MetodoPago>> ObtenerPorUsuarioIdAsync(int usuarioId)
            => _contexto.MetodosPago.Where(m => m.UsuarioId == usuarioId).ToListAsync();
    }
}

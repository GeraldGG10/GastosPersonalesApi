using GastosPersonales.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GastosPersonales.Domain.Interfaces
{
    public interface IUsuarioRepositorio
    {
        Task<Usuario?> ObtenerPorCorreoAsync(string correo);
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Usuario usuario);
        Task ActualizarAsync(Usuario usuario);
    }

    public interface ICategoriaRepositorio
    {
        Task AgregarAsync(Categoria categoria);
        Task<List<Categoria>> ObtenerPorUsuarioIdAsync(int usuarioId);
        Task<Categoria?> ObtenerPorIdAsync(int id);
        Task ActualizarAsync(Categoria categoria);
        Task EliminarAsync(Categoria categoria);
    }

    public interface IMetodoPagoRepositorio
    {
        Task AgregarAsync(MetodoPago metodo);
        Task<System.Collections.Generic.List<MetodoPago>> ObtenerPorUsuarioIdAsync(int usuarioId);
        Task<MetodoPago?> ObtenerPorIdAsync(int id);
        Task ActualizarAsync(MetodoPago metodo);
        Task EliminarAsync(MetodoPago metodo);
    }

    public interface IGastoRepositorio
    {
        Task AgregarAsync(Gasto gasto);
        Task<System.Collections.Generic.List<Gasto>> ObtenerPorUsuarioIdAsync(int usuarioId);
    }

    public interface IBudgetRepository
    {
        Task AddAsync(Budget budget);
        Task<List<Budget>> GetByUserIdAsync(int userId);
        Task<Budget?> GetByIdAsync(int id);
        Task UpdateAsync(Budget budget);
        Task DeleteAsync(Budget budget);
    }

    public interface IGeneradorJwt
    {
        string GenerarToken(int usuarioId, string correo);
    }
}

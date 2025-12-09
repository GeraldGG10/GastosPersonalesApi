using GastosPersonales.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GastosPersonales.Domain.Interfaces
{
    // Interfaces para los repositorios de las entidades del dominio
    public interface IUsuarioRepositorio
    {
        Task<Usuario?> ObtenerPorCorreoAsync(string correo);
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Usuario usuario);
        Task ActualizarAsync(Usuario usuario);
    }
    //Interface para el repositorio de Categoria
    public interface ICategoriaRepositorio
    {
        Task AgregarAsync(Categoria categoria);
        Task<List<Categoria>> ObtenerPorUsuarioIdAsync(int usuarioId);
        Task<Categoria?> ObtenerPorIdAsync(int id);
        Task ActualizarAsync(Categoria categoria);
        Task EliminarAsync(Categoria categoria);
    }

    //Interface para el repositorio de MetodoPago
    public interface IMetodoPagoRepositorio
    {
        Task AgregarAsync(MetodoPago metodo);
        Task<System.Collections.Generic.List<MetodoPago>> ObtenerPorUsuarioIdAsync(int usuarioId);
        Task<MetodoPago?> ObtenerPorIdAsync(int id);
        Task ActualizarAsync(MetodoPago metodo);
        Task EliminarAsync(MetodoPago metodo);
    }

    //Interface para el repositorio de Gasto
    public interface IGastoRepositorio
    {
        Task AgregarAsync(Gasto gasto);
        Task<System.Collections.Generic.List<Gasto>> ObtenerPorUsuarioIdAsync(int usuarioId);
    }

    //Interface para el repositorio de Presupuesto
    public interface IBudgetRepository
    {
        Task AddAsync(Budget budget);
        Task<List<Budget>> GetByUserIdAsync(int userId);
        Task<Budget?> GetByIdAsync(int id);
        Task UpdateAsync(Budget budget);
        Task DeleteAsync(Budget budget);
    }

    //Interface para el generador de JWT
    public interface IGeneradorJwt
    {
        string GenerarToken(int usuarioId, string correo);
    }
}

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
    }

    public interface ICategoriaRepositorio
    {
        Task AgregarAsync(Categoria categoria);
        Task<List<Categoria>> ObtenerPorUsuarioIdAsync(int usuarioId);
    }

    public interface IMetodoPagoRepositorio
    {
        Task AgregarAsync(MetodoPago metodo);
        Task<System.Collections.Generic.List<MetodoPago>> ObtenerPorUsuarioIdAsync(int usuarioId);
    }

    public interface IGastoRepositorio
    {
        Task AgregarAsync(Gasto gasto);
        Task<System.Collections.Generic.List<Gasto>> ObtenerPorUsuarioIdAsync(int usuarioId);
    }

    public interface IGeneradorJwt
    {
        string GenerarToken(int usuarioId, string correo);
    }
}

using GastosPersonales.Application.Models;

namespace GastosPersonales.Application.Services.Interfaces
{
    // Interfaz para el servicio de gestión de métodos de pago
    public interface IMethodService
    {
        Task<IEnumerable<PaymentMethod>> GetAll(int userId);
        Task<PaymentMethod> GetById(int id, int userId);
        Task<PaymentMethod> Create(PaymentMethodDTO dto, int userId);
        Task<PaymentMethod> Update(int id, PaymentMethodDTO dto, int userId);
        Task<bool> Delete(int id, int userId);
    }
}

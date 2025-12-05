using GastosPersonales.Application.Models;

namespace GastosPersonales.Application.Services.Interfaces
{
    public interface IMethodService
    {
        Task<IEnumerable<PaymentMethod>> GetAll(int userId);
        Task<PaymentMethod> GetById(int id, int userId);
        Task<PaymentMethod> Create(PaymentMethodDTO dto, int userId);
        Task<PaymentMethod> Update(int id, PaymentMethodDTO dto, int userId);
        Task<bool> Delete(int id, int userId);
    }
}

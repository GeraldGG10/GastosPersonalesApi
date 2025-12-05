using GastosPersonales.Application.Models;
using GastosPersonales.Application.Services.Interfaces;

namespace GastosPersonales.Application.Services.Implementations
{
    public class MethodService : IMethodService
    {
        private static List<PaymentMethod> _methods = new List<PaymentMethod>();
        private static int _nextId = 1;

        public async Task<PaymentMethod> Create(PaymentMethodDTO dto, int userId)
        {
            var method = new PaymentMethod
            {
                Id = _nextId++,
                Name = dto.Name,
                Icon = dto.Icon,
                IsActive = dto.IsActive,
                UserId = userId
            };
            _methods.Add(method);
            return await Task.FromResult(method);
        }

        public async Task<bool> Delete(int id, int userId)
        {
            var method = _methods.FirstOrDefault(m => m.Id == id && m.UserId == userId);
            if(method == null) return await Task.FromResult(false);
            _methods.Remove(method);
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<PaymentMethod>> GetAll(int userId)
        {
            return await Task.FromResult(_methods.Where(m => m.UserId == userId));
        }

        public async Task<PaymentMethod> GetById(int id, int userId)
        {
            return await Task.FromResult(_methods.FirstOrDefault(m => m.Id == id && m.UserId == userId));
        }

        public async Task<PaymentMethod> Update(int id, PaymentMethodDTO dto, int userId)
        {
            var method = _methods.FirstOrDefault(m => m.Id == id && m.UserId == userId);
            if(method != null)
            {
                method.Name = dto.Name;
                method.Icon = dto.Icon;
                method.IsActive = dto.IsActive;
            }
            return await Task.FromResult(method);
        }
    }
}

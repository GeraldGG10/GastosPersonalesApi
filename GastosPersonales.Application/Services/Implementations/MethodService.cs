using GastosPersonales.Application.Models;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GastosPersonales.Application.Services.Implementations
{
    public class MethodService : IMethodService
    {
        private readonly IMetodoPagoRepositorio _repositorio;

        public MethodService(IMetodoPagoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<PaymentMethod> Create(PaymentMethodDTO dto, int userId)
        {
            var metodo = new MetodoPago
            {
                Nombre = dto.Name,
                UsuarioId = userId
                // La entidad no tiene Icon ni IsActive.
            };

            await _repositorio.AgregarAsync(metodo);

            return new PaymentMethod
            {
                Id = metodo.Id,
                Name = metodo.Nombre,
                Icon = dto.Icon,
                IsActive = true,
                UserId = metodo.UsuarioId
            };
        }

        public async Task<bool> Delete(int id, int userId)
        {
            var metodo = await _repositorio.ObtenerPorIdAsync(id);
            if (metodo == null || metodo.UsuarioId != userId) return false;

            await _repositorio.EliminarAsync(metodo);
            return true;
        }

        public async Task<IEnumerable<PaymentMethod>> GetAll(int userId)
        {
            var metodos = await _repositorio.ObtenerPorUsuarioIdAsync(userId);
            var result = new List<PaymentMethod>();
            foreach (var m in metodos)
            {
                result.Add(new PaymentMethod
                {
                    Id = m.Id,
                    Name = m.Nombre,
                    Icon = "credit-card", // Default icon as it is not stored
                    IsActive = true,
                    UserId = m.UsuarioId
                });
            }
            return result;
        }

        public async Task<PaymentMethod> GetById(int id, int userId)
        {
            var m = await _repositorio.ObtenerPorIdAsync(id);
            if (m == null || m.UsuarioId != userId) return null;

            return new PaymentMethod
            {
                Id = m.Id,
                Name = m.Nombre,
                Icon = "credit-card",
                IsActive = true,
                UserId = m.UsuarioId
            };
        }

        public async Task<PaymentMethod> Update(int id, PaymentMethodDTO dto, int userId)
        {
            var m = await _repositorio.ObtenerPorIdAsync(id);
            if (m == null || m.UsuarioId != userId) return null;

            m.Nombre = dto.Name;
            await _repositorio.ActualizarAsync(m);

            return new PaymentMethod
            {
                Id = m.Id,
                Name = m.Nombre,
                Icon = dto.Icon,
                IsActive = true,
                UserId = m.UsuarioId
            };
        }
    }
}

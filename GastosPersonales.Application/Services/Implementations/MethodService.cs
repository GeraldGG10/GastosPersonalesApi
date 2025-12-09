using GastosPersonales.Application.Models;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GastosPersonales.Application.Services.Implementations
{
    // Servicio para la gestión de métodos de pago
    public class MethodService : IMethodService
    {
        private readonly IMetodoPagoRepositorio _repositorio;

        // Constructor que recibe el repositorio de métodos de pago
        public MethodService(IMetodoPagoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        // Creacion de un nuevo método de pago
        public async Task<PaymentMethod> Create(PaymentMethodDTO dto, int userId)
        {
            var metodo = new MetodoPago
            {
                Nombre = dto.Name,
                UsuarioId = userId
                
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

        // Borrar métodos de pago por ID y usuario
        public async Task<bool> Delete(int id, int userId)
        {
            var metodo = await _repositorio.ObtenerPorIdAsync(id);
            if (metodo == null || metodo.UsuarioId != userId) return false;

            await _repositorio.EliminarAsync(metodo);
            return true;
        }

        // Obtener todos los métodos de pago para un usuario específico
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
                    Icon = "credit-card", 
                    IsActive = true,
                    UserId = m.UsuarioId
                });
            }
            return result;
        }

        // Obtener métodos de pago por ID y usuario
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

        // Actualizar los métodos de pago por ID y usuario
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

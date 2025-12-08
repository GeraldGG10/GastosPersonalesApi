using GastosPersonales.Application.Models;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GastosPersonales.Application.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoriaRepositorio _repositorio;

        public CategoryService(ICategoriaRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<Category> Create(CategoryDTO dto, int userId)
        {
            var category = new Categoria
            {
                Nombre = dto.Name,
                UsuarioId = userId,
                // IsActive se asume true o se agrega a la entidad si es necesario. 
                // Revisando entidad Categoria: public string Nombre { get; set; } = ""; public int UsuarioId { get; set; }
                // La entidad no tiene IsActive actualmente. Lo omitiremos o agregaremos en un futuro si se pide explicitamente.
            };

            await _repositorio.AgregarAsync(category);

            // Mapear de vuelta a modelo de aplicación (si es necesario) o retornar DTO/Entidad mapeada
            return new Category
            {
                Id = category.Id,
                Name = category.Nombre,
                IsActive = true, // Default
                UserId = category.UsuarioId
            };
        }

        public async Task<bool> Delete(int id, int userId)
        {
            var cat = await _repositorio.ObtenerPorIdAsync(id);
            if (cat == null || cat.UsuarioId != userId) return false;

            await _repositorio.EliminarAsync(cat);
            return true;
        }

        public async Task<IEnumerable<Category>> GetAll(int userId)
        {
            var categorias = await _repositorio.ObtenerPorUsuarioIdAsync(userId);
            // Mapear entidad a modelo
            var result = new List<Category>();
            foreach (var c in categorias)
            {
                result.Add(new Category
                {
                    Id = c.Id,
                    Name = c.Nombre,
                    IsActive = true,
                    UserId = c.UsuarioId
                });
            }
            return result;
        }

        public async Task<Category> GetById(int id, int userId)
        {
            var cat = await _repositorio.ObtenerPorIdAsync(id);
            if (cat == null || cat.UsuarioId != userId) return null;

            return new Category
            {
                Id = cat.Id,
                Name = cat.Nombre,
                IsActive = true,
                UserId = cat.UsuarioId
            };
        }

        public async Task<Category> Update(int id, CategoryDTO dto, int userId)
        {
            var cat = await _repositorio.ObtenerPorIdAsync(id);
            if (cat == null || cat.UsuarioId != userId) return null;

            cat.Nombre = dto.Name;
            await _repositorio.ActualizarAsync(cat);

            return new Category
            {
                Id = cat.Id,
                Name = cat.Nombre,
                IsActive = true,
                UserId = cat.UsuarioId
            };
        }
    }
}

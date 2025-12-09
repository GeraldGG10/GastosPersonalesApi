using GastosPersonales.Application.Models;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GastosPersonales.Application.Services.Implementations
{
    // Servicio para la gestión de categorías
    public class CategoryService : ICategoryService
    {
        private readonly ICategoriaRepositorio _repositorio;

        // Constructor que inyecta el repositorio de categorías
        public CategoryService(ICategoriaRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        // Crear una nueva categoría
        public async Task<Category> Create(CategoryDTO dto, int userId)
        {
            var category = new Categoria
            {
                Nombre = dto.Name,
                UsuarioId = userId,
                
            };

            await _repositorio.AgregarAsync(category);

            
            return new Category
            {
                Id = category.Id,
                Name = category.Nombre,
                IsActive = true, // Default
                UserId = category.UsuarioId
            };
        }

        //Borrar una categoría por su ID
        public async Task<bool> Delete(int id, int userId)
        {
            var cat = await _repositorio.ObtenerPorIdAsync(id);
            if (cat == null || cat.UsuarioId != userId) return false;

            await _repositorio.EliminarAsync(cat);
            return true;
        }

        // Obtener todas las categorías de un usuario
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

        // Obtener una categoría por su ID
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

        // Actualizar una categoría existente
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

using GastosPersonales.Application.Models;
using GastosPersonales.Application.Services.Interfaces;

namespace GastosPersonales.Application.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private static List<Category> _categories = new List<Category>();
        private static int _nextId = 1;

        public async Task<Category> Create(CategoryDTO dto, int userId)
        {
            var category = new Category
            {
                Id = _nextId++,
                Name = dto.Name,
                IsActive = dto.IsActive,
                UserId = userId
            };
            _categories.Add(category);
            return await Task.FromResult(category);
        }

        public async Task<bool> Delete(int id, int userId)
        {
            var cat = _categories.FirstOrDefault(c => c.Id == id && c.UserId == userId);
            if(cat == null) return await Task.FromResult(false);
            _categories.Remove(cat);
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<Category>> GetAll(int userId)
        {
            return await Task.FromResult(_categories.Where(c => c.UserId == userId));
        }

        public async Task<Category> GetById(int id, int userId)
        {
            return await Task.FromResult(_categories.FirstOrDefault(c => c.Id == id && c.UserId == userId));
        }

        public async Task<Category> Update(int id, CategoryDTO dto, int userId)
        {
            var cat = _categories.FirstOrDefault(c => c.Id == id && c.UserId == userId);
            if(cat != null)
            {
                cat.Name = dto.Name;
                cat.IsActive = dto.IsActive;
            }
            return await Task.FromResult(cat);
        }
    }
}

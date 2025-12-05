using GastosPersonales.Application.Models;

namespace GastosPersonales.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAll(int userId);
        Task<Category> GetById(int id, int userId);
        Task<Category> Create(CategoryDTO dto, int userId);
        Task<Category> Update(int id, CategoryDTO dto, int userId);
        Task<bool> Delete(int id, int userId);
    }
}

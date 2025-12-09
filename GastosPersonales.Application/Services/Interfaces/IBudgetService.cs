using GastosPersonales.Application.Models;

namespace GastosPersonales.Application.Services.Interfaces
{
    // Interfaz para el servicio de gestión de presupuestos
    public interface IBudgetService
    {
        Task<IEnumerable<Budget>> GetAll(int userId);
        Task<Budget> GetByCategoryMonth(int categoryId, int month, int year, int userId);
        Task<Budget> Create(BudgetDTO dto, int userId);
        Task<Budget> Update(int id, BudgetDTO dto, int userId);
        Task<bool> Delete(int id, int userId);
        Task<decimal> CalculateSpentPercentage(int categoryId, int month, int year, int userId);
        Task<IEnumerable<object>> GetExceededBudgets(int month, int year, int userId);
    }
}

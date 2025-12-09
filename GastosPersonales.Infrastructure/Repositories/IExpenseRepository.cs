using GastosPersonales.Domain.Entities;

namespace GastosPersonales.Infrastructure.Repositories
{
    // Repositorio para la gestión de gastos
    public interface IExpenseRepository
    {
        Task<IEnumerable<Expense>> GetByUserId(int userId);
        Task<Expense?> GetById(int id, int userId);
        Task<Expense> Add(Expense expense);
        Task<Expense> Update(Expense expense);
        Task<bool> Delete(int id, int userId);
        Task<IEnumerable<Expense>> Filter(DateTime? startDate, DateTime? endDate, 
            int? categoryId, int? paymentMethodId, string? search, int userId);
    }
}

using GastosPersonales.Application.Models;

namespace GastosPersonales.Application.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<IEnumerable<Expense>> GetAll(int userId);
        Task<Expense> GetById(int id, int userId);
        Task<Expense> Create(ExpenseDTO dto, int userId);
        Task<Expense> Update(int id, ExpenseDTO dto, int userId);
        Task<bool> Delete(int id, int userId);
    }
}

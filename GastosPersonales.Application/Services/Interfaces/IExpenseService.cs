using GastosPersonales.Application.Models;
using GastosPersonales.Application.DTOs;
using GastosPersonales.Domain.Entities;

namespace GastosPersonales.Application.Services.Interfaces
{
    // Interfaz para la gestión de gastos
    public interface IExpenseService
    {
        Task<IEnumerable<Expense>> GetAll(int userId);
        Task<Expense> GetById(int id, int userId);
        Task<Expense> Create(ExpenseDTO dto, int userId);
        Task<Expense> Update(int id, ExpenseDTO dto, int userId);
        Task<bool> Delete(int id, int userId);
        Task<ImportResultDTO> ImportFromExcel(Stream fileStream, string fileName, int userId);
    }
}

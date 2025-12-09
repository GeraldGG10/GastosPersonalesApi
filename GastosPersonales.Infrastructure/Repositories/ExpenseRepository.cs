using GastosPersonales.Domain.Entities;
using GastosPersonales.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace GastosPersonales.Infrastructure.Repositories
{
    // Repositorio para la gestión de gastos
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly AplicacionDbContext _context;
        // Constructor que recibe el contexto de la base de datos
        public ExpenseRepository(AplicacionDbContext context)
        {
            _context = context;
        }

        // Obtener todos los gastos de un usuario específico
        public async Task<IEnumerable<Expense>> GetByUserId(int userId)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        // Obtener un gasto por su ID y el ID del usuario
        public async Task<Expense?> GetById(int id, int userId)
        {
            return await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        }

        // Agregar un nuevo gasto
        public async Task<Expense> Add(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return expense;
        }

        // Actualizar un gasto existente
        public async Task<Expense> Update(Expense expense)
        {
            _context.Expenses.Update(expense);
            await _context.SaveChangesAsync();
            return expense;
        }

        // Eliminar un gasto por su ID y el ID del usuario
        public async Task<bool> Delete(int id, int userId)
        {
            var expense = await GetById(id, userId);
            if (expense == null) return false;

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }

        // Filtrar gastos según varios criterios
        public async Task<IEnumerable<Expense>> Filter(DateTime? startDate, DateTime? endDate, 
            int? categoryId, int? paymentMethodId, string? search, int userId)
        {
            var query = _context.Expenses.Where(e => e.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(e => e.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.Date <= endDate.Value);

            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryId == categoryId.Value);

            if (paymentMethodId.HasValue)
                query = query.Where(e => e.PaymentMethodId == paymentMethodId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.Description.Contains(search));

            return await query.OrderByDescending(e => e.Date).ToListAsync();
        }
    }
}

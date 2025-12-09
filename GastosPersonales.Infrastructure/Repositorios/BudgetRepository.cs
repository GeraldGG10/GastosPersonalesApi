using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Interfaces;
using GastosPersonales.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GastosPersonales.Infrastructure.Repositorios
{
    // Implementación del repositorio para la entidad Budget
    public class BudgetRepository : IBudgetRepository
    {
        private readonly AplicacionDbContext _context;

        public BudgetRepository(AplicacionDbContext context)
        {
            _context = context;
        }

        // Agrega un nuevo presupuesto
        public async Task AddAsync(Budget budget)
        {
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
        }

        // Elimina un presupuesto existente
        public async Task DeleteAsync(Budget budget)
        {
            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
        }

        // Obtiene todos los presupuestos de un usuario específico
        public async Task<List<Budget>> GetByUserIdAsync(int userId)
        {
            return await _context.Budgets.Where(b => b.UserId == userId).ToListAsync();
        }

        // Obtiene un presupuesto por su ID
        public async Task<Budget?> GetByIdAsync(int id)
        {
            return await _context.Budgets.FindAsync(id);
        }

        // Actualiza un presupuesto existente
        public async Task UpdateAsync(Budget budget)
        {
            _context.Budgets.Update(budget);
            await _context.SaveChangesAsync();
        }
    }
}

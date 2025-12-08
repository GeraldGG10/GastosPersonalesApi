using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Interfaces;
using GastosPersonales.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GastosPersonales.Infrastructure.Repositorios
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly AplicacionDbContext _context;

        public BudgetRepository(AplicacionDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Budget budget)
        {
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Budget budget)
        {
            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Budget>> GetByUserIdAsync(int userId)
        {
            return await _context.Budgets.Where(b => b.UserId == userId).ToListAsync();
        }

        public async Task<Budget?> GetByIdAsync(int id)
        {
            return await _context.Budgets.FindAsync(id);
        }

        public async Task UpdateAsync(Budget budget)
        {
            _context.Budgets.Update(budget);
            await _context.SaveChangesAsync();
        }
    }
}

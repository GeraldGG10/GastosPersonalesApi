using GastosPersonales.Application.Models;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Services.Implementations;

namespace GastosPersonales.Application.Services.Implementations
{
    public class BudgetService : IBudgetService
    {
        private static List<Budget> _budgets = new List<Budget>();
        private static int _nextId = 1;

        private readonly ExpenseService _expenseService;

        public BudgetService()
        {
            _expenseService = new ExpenseService();
        }

        public async Task<Budget> Create(BudgetDTO dto, int userId)
        {
            var budget = new Budget
            {
                Id = _nextId++,
                CategoryId = dto.CategoryId,
                Amount = dto.Amount,
                Month = dto.Month,
                Year = dto.Year,
                UserId = userId
            };
            _budgets.Add(budget);
            return await Task.FromResult(budget);
        }

        public async Task<bool> Delete(int id, int userId)
        {
            var budget = _budgets.FirstOrDefault(b => b.Id == id && b.UserId == userId);
            if(budget == null) return await Task.FromResult(false);
            _budgets.Remove(budget);
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<Budget>> GetAll(int userId)
        {
            return await Task.FromResult(_budgets.Where(b => b.UserId == userId));
        }

        public async Task<Budget> GetByCategoryMonth(int categoryId, int month, int year, int userId)
        {
            return await Task.FromResult(_budgets.FirstOrDefault(b => b.CategoryId == categoryId && b.Month == month && b.Year == year && b.UserId == userId));
        }

        public async Task<Budget> Update(int id, BudgetDTO dto, int userId)
        {
            var budget = _budgets.FirstOrDefault(b => b.Id == id && b.UserId == userId);
            if(budget != null)
            {
                budget.CategoryId = dto.CategoryId;
                budget.Amount = dto.Amount;
                budget.Month = dto.Month;
                budget.Year = dto.Year;
            }
            return await Task.FromResult(budget);
        }

        public async Task<decimal> CalculateSpentPercentage(int categoryId, int month, int year, int userId)
        {
            var budget = _budgets.FirstOrDefault(b => b.CategoryId == categoryId && b.Month == month && b.Year == year && b.UserId == userId);
            if(budget == null) return 0;

            var expenses = await _expenseService.GetAll(userId);
            var spent = expenses.Where(e => e.CategoryId == categoryId && e.Date.Month == month && e.Date.Year == year).Sum(e => e.Amount);

            var percentage = (spent / budget.Amount) * 100;
            return Math.Round(percentage,2);
        }
    }
}

using GastosPersonales.Application.Models;
using GastosPersonales.Application.Services.Interfaces;
using System.Linq;

namespace GastosPersonales.Application.Services.Implementations
{
    public class BudgetService : IBudgetService
    {
        private static List<Budget> _budgets = new List<Budget>();
        private static int _nextId = 1;
        private readonly IExpenseService _expenseService;

        public BudgetService(IExpenseService expenseService)
        {
            _expenseService = expenseService;
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

        public async Task<IEnumerable<Budget>> GetAll(int userId)
        {
            return await Task.FromResult(_budgets.Where(b => b.UserId == userId));
        }

        public async Task<Budget> GetByCategoryMonth(int categoryId, int month, int year, int userId)
        {
            var budget = _budgets.FirstOrDefault(b => 
                b.CategoryId == categoryId && 
                b.Month == month && 
                b.Year == year && 
                b.UserId == userId);

            if (budget == null)
                throw new KeyNotFoundException($"Budget not found for category {categoryId}, month {month}, year {year}");

            return await Task.FromResult(budget);
        }

        public async Task<Budget> Update(int id, BudgetDTO dto, int userId)
        {
            var budget = _budgets.FirstOrDefault(b => b.Id == id && b.UserId == userId);
            if (budget == null) 
                throw new KeyNotFoundException($"Budget with id {id} not found");

            budget.CategoryId = dto.CategoryId;
            budget.Amount = dto.Amount;
            budget.Month = dto.Month;
            budget.Year = dto.Year;

            return await Task.FromResult(budget);
        }

        public async Task<bool> Delete(int id, int userId)
        {
            var budget = _budgets.FirstOrDefault(b => b.Id == id && b.UserId == userId);
            if (budget == null) return false;

            _budgets.Remove(budget);
            return await Task.FromResult(true);
        }

        public async Task<decimal> CalculateSpentPercentage(int categoryId, int month, int year, int userId)
        {
            var budget = _budgets.FirstOrDefault(b => 
                b.CategoryId == categoryId && 
                b.Month == month && 
                b.Year == year && 
                b.UserId == userId);

            if (budget == null || budget.Amount == 0)
                return 0;

            var expenses = await _expenseService.GetAll(userId);
            var categoryExpenses = expenses
                .Where(e => e.CategoryId == categoryId && 
                           e.Date.Month == month && 
                           e.Date.Year == year)
                .Sum(e => e.Amount);

            return (categoryExpenses / budget.Amount) * 100;
        }

        public async Task<IEnumerable<object>> GetExceededBudgets(int month, int year, int userId)
        {
            var userBudgets = _budgets.Where(b => 
                b.Month == month && 
                b.Year == year && 
                b.UserId == userId);

            var expenses = await _expenseService.GetAll(userId);
            
            var exceededList = new List<object>();

            foreach (var budget in userBudgets)
            {
                var categoryExpenses = expenses
                    .Where(e => e.CategoryId == budget.CategoryId && 
                               e.Date.Month == month && 
                               e.Date.Year == year)
                    .Sum(e => e.Amount);

                var percentage = budget.Amount > 0 ? (categoryExpenses / budget.Amount) * 100 : 0;

                string status;
                if (percentage >= 100)
                    status = "EXCEDIDO";
                else if (percentage >= 80)
                    status = "CRÍTICO";
                else if (percentage >= 50)
                    status = "ADVERTENCIA";
                else
                    status = "OK";

                exceededList.Add(new
                {
                    CategoryId = budget.CategoryId,
                    BudgetAmount = budget.Amount,
                    SpentAmount = categoryExpenses,
                    Percentage = Math.Round(percentage, 2),
                    Status = status,
                    Remaining = budget.Amount - categoryExpenses
                });
            }

            // Retornar solo los que están en advertencia o peor
            return exceededList
                .Where(b => ((dynamic)b).Percentage >= 50)
                .OrderByDescending(b => ((dynamic)b).Percentage);
        }
    }
}

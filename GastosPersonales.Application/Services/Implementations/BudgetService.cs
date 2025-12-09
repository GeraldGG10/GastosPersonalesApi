using GastosPersonales.Application.Models;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using DomainBudget = GastosPersonales.Domain.Entities.Budget;
using AppBudget = GastosPersonales.Application.Models.Budget;

namespace GastosPersonales.Application.Services.Implementations
{
    // Servicio para la gestión de presupuestos 
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _repository;
        private readonly IExpenseService _expenseService;

        // Constructor del servicio de presupuesto
        public BudgetService(IBudgetRepository repository, IExpenseService expenseService)
        {
            _repository = repository;
            _expenseService = expenseService;
        }

        // Crear un nuevo presupuesto
        public async Task<AppBudget> Create(BudgetDTO dto, int userId)
        {
            var budgetEntity = new DomainBudget
            {
                CategoryId = dto.CategoryId,
                Amount = dto.Amount,
                Month = dto.Month,
                Year = dto.Year,
                UserId = userId
            };

            await _repository.AddAsync(budgetEntity);

            return MapToModel(budgetEntity);
        }

        // Obtener todos los presupuestos de un usuario
        public async Task<IEnumerable<AppBudget>> GetAll(int userId)
        {
            var budgets = await _repository.GetByUserIdAsync(userId);
            return budgets.Select(MapToModel);
        }


        // Obtener un presupuesto por categoría, mes y año
        public async Task<AppBudget> GetByCategoryMonth(int categoryId, int month, int year, int userId)
        {
            var budgets = await _repository.GetByUserIdAsync(userId);
            var budget = budgets.FirstOrDefault(b =>
                b.CategoryId == categoryId &&
                b.Month == month &&
                b.Year == year);

            if (budget == null)
                throw new KeyNotFoundException($"Budget not found for category {categoryId}, month {month}, year {year}");

            return MapToModel(budget);
        }

        // Actualizar un presupuesto existente
        public async Task<AppBudget> Update(int id, BudgetDTO dto, int userId)
        {
            var budget = await _repository.GetByIdAsync(id);
            if (budget == null || budget.UserId != userId)
                throw new KeyNotFoundException($"Budget with id {id} not found");

            budget.CategoryId = dto.CategoryId;
            budget.Amount = dto.Amount;
            budget.Month = dto.Month;
            budget.Year = dto.Year;

            await _repository.UpdateAsync(budget);

            return MapToModel(budget);
        }

        // Eliminar 
        public async Task<bool> Delete(int id, int userId)
        {
            var budget = await _repository.GetByIdAsync(id);
            if (budget == null || budget.UserId != userId) return false;

            await _repository.DeleteAsync(budget);
            return true;
        }

        // Calcular el porcentaje gastado en una categoría específica para un mes y año dados
        public async Task<decimal> CalculateSpentPercentage(int categoryId, int month, int year, int userId)
        {
            var budgets = await _repository.GetByUserIdAsync(userId);
            var budget = budgets.FirstOrDefault(b =>
                b.CategoryId == categoryId &&
                b.Month == month &&
                b.Year == year);

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

        // Obtener la lista de presupuestos que han sido excedidos o están en estado crítico
        public async Task<IEnumerable<object>> GetExceededBudgets(int month, int year, int userId)
        {
            var allBudgets = await _repository.GetByUserIdAsync(userId);
            var userBudgets = allBudgets.Where(b =>
                b.Month == month &&
                b.Year == year).ToList();

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
                    Percentage = System.Math.Round(percentage, 2),
                    Status = status,
                    Remaining = budget.Amount - categoryExpenses
                });
            }

            return exceededList
                .Where(b => ((dynamic)b).Percentage >= 50)
                .OrderByDescending(b => ((dynamic)b).Percentage);
        }

        // Mapeo de la entidad de dominio al modelo de aplicación
        private AppBudget MapToModel(DomainBudget entity)
        {
            return new AppBudget
            {
                Id = entity.Id,
                CategoryId = entity.CategoryId,
                Amount = entity.Amount,
                Month = entity.Month,
                Year = entity.Year,
                UserId = entity.UserId
            };
        }
    }
}

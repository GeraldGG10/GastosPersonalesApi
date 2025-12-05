using System.Collections.Generic;
using System.Threading.Tasks;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Infrastructure.Repositories;

namespace GastosPersonales.Application.Services.Implementations
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;

        public ExpenseService(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public async Task<IEnumerable<Expense>> GetAll(int userId)
        {
            return await _expenseRepository.GetByUserId(userId);
        }

        public async Task<Expense> GetById(int id, int userId)
        {
            return await _expenseRepository.GetById(id, userId);
        }

        public async Task Create(ExpenseDTO expenseDto, int userId)
        {
            var expense = new Expense
            {
                UserId = userId,
                Description = expenseDto.Description,
                Amount = expenseDto.Amount,
                Date = expenseDto.Date
            };
            await _expenseRepository.Add(expense);
        }

        public async Task Update(int id, ExpenseDTO expenseDto, int userId)
        {
            var expense = await _expenseRepository.GetById(id, userId);
            if (expense == null) throw new KeyNotFoundException();
            expense.Description = expenseDto.Description;
            expense.Amount = expenseDto.Amount;
            expense.Date = expenseDto.Date;
            await _expenseRepository.Update(expense);
        }

        public async Task Delete(int id, int userId)
        {
            await _expenseRepository.Delete(id, userId);
        }
    }
}

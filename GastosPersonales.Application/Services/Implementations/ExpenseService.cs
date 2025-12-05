using System.Collections.Generic;
using System.Threading.Tasks;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Models;
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
            var expense = await _expenseRepository.GetById(id, userId);
            if (expense == null) throw new KeyNotFoundException($"Expense with id {id} not found");
            return expense;
        }

        public async Task<Expense> Create(ExpenseDTO expenseDto, int userId)
        {
            var expense = new Expense
            {
                UserId = userId,
                Description = expenseDto.Description,
                Amount = expenseDto.Amount,
                Date = expenseDto.Date,
                CategoryId = expenseDto.CategoryId,
                PaymentMethodId = expenseDto.PaymentMethodId
            };
            return await _expenseRepository.Add(expense);
        }

        public async Task<Expense> Update(int id, ExpenseDTO expenseDto, int userId)
        {
            var expense = await _expenseRepository.GetById(id, userId);
            if (expense == null) throw new KeyNotFoundException($"Expense with id {id} not found");
            
            expense.Description = expenseDto.Description;
            expense.Amount = expenseDto.Amount;
            expense.Date = expenseDto.Date;
            expense.CategoryId = expenseDto.CategoryId;
            expense.PaymentMethodId = expenseDto.PaymentMethodId;
            
            return await _expenseRepository.Update(expense);
        }

        public async Task<bool> Delete(int id, int userId)
        {
            return await _expenseRepository.Delete(id, userId);
        }
    }
}

using GastosPersonales.Domain.Entities;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.DTOs;
using GastosPersonales.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            return await _expenseRepository.GetAllAsync(userId);
        }

        public async Task<Expense> GetById(int id, int userId)
        {
            return await _expenseRepository.GetByIdAsync(id, userId);
        }

        public async Task Create(ExpenseDTO dto, int userId)
        {
            var expense = new Expense
            {
                Amount = dto.Amount,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                PaymentMethodId = dto.PaymentMethodId,
                UserId = userId,
                Date = dto.Date
            };

            await _expenseRepository.CreateAsync(expense);
        }

        public async Task Update(int id, ExpenseDTO dto, int userId)
        {
            var expense = await _expenseRepository.GetByIdAsync(id, userId);
            if (expense == null) throw new Exception("Expense not found");

            expense.Amount = dto.Amount;
            expense.Description = dto.Description;
            expense.CategoryId = dto.CategoryId;
            expense.PaymentMethodId = dto.PaymentMethodId;
            expense.Date = dto.Date;

            await _expenseRepository.UpdateAsync(expense);
        }

        public async Task<bool> Delete(int id, int userId)
        {
            var expense = await _expenseRepository.GetByIdAsync(id, userId);
            if (expense == null) return false;

            await _expenseRepository.DeleteAsync(expense);
            return true;
        }
    }
}

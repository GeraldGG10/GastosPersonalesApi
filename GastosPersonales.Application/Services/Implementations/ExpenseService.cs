using GastosPersonales.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GastosPersonales.Application.Models;
using GastosPersonales.Application.Services.Interfaces;
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
            return await _expenseRepository.GetAll(userId);
        }

        public async Task<Expense> GetById(int id, int userId)
        {
            return await _expenseRepository.GetById(id, userId);
        }

        public async Task<Expense> Create(ExpenseDTO expenseDTO, int userId)
        {
            var expense = new Expense
            {
                UserId = userId,
                Amount = expenseDTO.Amount,
                Description = expenseDTO.Description,
                Date = expenseDTO.Date
            };
            return await _expenseRepository.Create(expense);
        }

        public async Task<Expense> Update(int id, ExpenseDTO expenseDTO, int userId)
        {
            var expense = await _expenseRepository.GetById(id, userId);
            if (expense == null) return null;

            expense.Amount = expenseDTO.Amount;
            expense.Description = expenseDTO.Description;
            expense.Date = expenseDTO.Date;

            return await _expenseRepository.Update(expense);
        }

        public async Task<bool> Delete(int id, int userId)
        {
            return await _expenseRepository.Delete(id, userId);
        }
    }
}


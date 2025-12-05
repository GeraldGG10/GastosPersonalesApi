using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Infrastructure.Repositories;

namespace GastosPersonales.Application.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IExpenseRepository _expenseRepository;

        public ReportService(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public async Task<IEnumerable<Expense>> FilterExpenses(DateTime? startDate, DateTime? endDate, int? categoryId, int? paymentMethodId, string? search, int userId)
        {
            return await _expenseRepository.Filter(startDate, endDate, categoryId, paymentMethodId, search, userId);
        }

        public async Task<string> ExportExpensesToTxt(int userId)
        {
            var expenses = await _expenseRepository.GetByUserId(userId);
            return "expenses.txt";
        }

        public async Task<string> ExportExpensesToJson(int userId)
        {
            var expenses = await _expenseRepository.GetByUserId(userId);
            return "expenses.json";
        }

        public Task<IEnumerable<Expense>> MonthlyReport(int year, int month, int userId)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Models;
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

        public async Task<object> MonthlyReport(int month, int year, int userId)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var expenses = await _expenseRepository.Filter(startDate, endDate, null, null, null, userId);
            
            var totalAmount = expenses.Sum(e => e.Amount);
            var expenseCount = expenses.Count();

            var byCategory = expenses
                .GroupBy(e => e.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    Total = g.Sum(e => e.Amount),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            return new
            {
                Year = year,
                Month = month,
                TotalAmount = totalAmount,
                ExpenseCount = expenseCount,
                ByCategory = byCategory
            };
        }
    }
}

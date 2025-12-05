using GastosPersonales.Domain.Entities;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var expenses = await _expenseRepository.GetAllAsync(userId);

            if (startDate.HasValue)
                expenses = expenses.Where(e => e.Date >= startDate.Value);

            if (endDate.HasValue)
                expenses = expenses.Where(e => e.Date <= endDate.Value);

            if (categoryId.HasValue)
                expenses = expenses.Where(e => e.CategoryId == categoryId.Value);

            if (paymentMethodId.HasValue)
                expenses = expenses.Where(e => e.PaymentMethodId == paymentMethodId.Value);

            if (!string.IsNullOrEmpty(search))
                expenses = expenses.Where(e => e.Description.Contains(search, StringComparison.OrdinalIgnoreCase));

            return expenses;
        }

        public Task<IEnumerable<Expense>> MonthlyReport(int month, int year, int userId)
        {
            return FilterExpenses(
                new DateTime(year, month, 1),
                new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                null,
                null,
                null,
                userId
            );
        }
    }
}

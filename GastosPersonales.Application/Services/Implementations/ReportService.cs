using GastosPersonales.Infrastructure;
using GastosPersonales.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GastosPersonales.Application.Models;
using GastosPersonales.Application.Services.Interfaces;
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

        public async Task<IEnumerable<Expense>> FilterExpenses(DateTime? startDate, DateTime? endDate, int? categoryId, int? userId, string description, int limit)
        {
            return await _expenseRepository.FilterExpenses(startDate, endDate, categoryId, userId, description, limit);
        }

        public async Task<object> MonthlyReport(int month, int year, int userId)
        {
            return await _expenseRepository.MonthlyReport(month, year, userId);
        }

        public Task ExportExpensesToTxt(int userId)
        {
            throw new NotImplementedException();
        }

        public Task ExportExpensesToJson(int userId)
        {
            throw new NotImplementedException();
        }
    }
}



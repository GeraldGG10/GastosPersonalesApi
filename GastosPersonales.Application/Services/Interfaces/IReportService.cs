using GastosPersonales.Application.Models;
using GastosPersonales.Domain.Entities;

namespace GastosPersonales.Application.Services.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<Expense>> FilterExpenses(DateTime? startDate, DateTime? endDate, int? categoryId, int? paymentMethodId, string? search, int userId);
        Task<object> MonthlyReport(int month, int year, int userId);
        Task<string> ExportExpensesToTxt(int userId);
        Task<string> ExportExpensesToJson(int userId);
        Task<byte[]> ExportExpensesToExcel(int userId);
    }
}

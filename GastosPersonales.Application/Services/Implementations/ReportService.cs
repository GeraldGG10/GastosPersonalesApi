using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Models;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Infrastructure.Repositories;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace GastosPersonales.Application.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IExpenseRepository _expenseRepository;

        public ReportService(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
            // Configurar licencia de EPPlus (NonCommercial)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<IEnumerable<Expense>> FilterExpenses(DateTime? startDate, DateTime? endDate, int? categoryId, int? paymentMethodId, string? search, int userId)
        {
            return await _expenseRepository.Filter(startDate, endDate, categoryId, paymentMethodId, search, userId);
        }

        public async Task<string> ExportExpensesToTxt(int userId)
        {
            var expenses = await _expenseRepository.GetByUserId(userId);
            
            var txt = "=== REPORTE DE GASTOS ===\n\n";
            txt += $"Total de gastos: {expenses.Count()}\n";
            txt += $"Total gastado: ${expenses.Sum(e => e.Amount):N2}\n\n";
            txt += "Detalle:\n";
            txt += "----------------------------------------\n";
            
            foreach (var expense in expenses.OrderByDescending(e => e.Date))
            {
                txt += $"{expense.Date:dd/MM/yyyy} | ${expense.Amount:N2} | {expense.Description}\n";
            }
            
            return txt;
        }

        public async Task<string> ExportExpensesToJson(int userId)
        {
            var expenses = await _expenseRepository.GetByUserId(userId);
            return System.Text.Json.JsonSerializer.Serialize(expenses, new System.Text.Json.JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }

        public async Task<byte[]> ExportExpensesToExcel(int userId)
        {
            var expenses = await _expenseRepository.GetByUserId(userId);
            
            using var package = new ExcelPackage();
            
            // Hoja 1: Lista de gastos
            var worksheet = package.Workbook.Worksheets.Add("Gastos");
            
            // Encabezados
            worksheet.Cells[1, 1].Value = "Fecha";
            worksheet.Cells[1, 2].Value = "Descripción";
            worksheet.Cells[1, 3].Value = "Monto";
            worksheet.Cells[1, 4].Value = "Categoría ID";
            worksheet.Cells[1, 5].Value = "Método Pago ID";
            
            // Estilo de encabezados
            using (var range = worksheet.Cells[1, 1, 1, 5])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
                range.Style.Font.Color.SetColor(Color.White);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            
            // Datos
            int row = 2;
            foreach (var expense in expenses.OrderByDescending(e => e.Date))
            {
                worksheet.Cells[row, 1].Value = expense.Date.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 2].Value = expense.Description;
                worksheet.Cells[row, 3].Value = expense.Amount;
                worksheet.Cells[row, 3].Style.Numberformat.Format = "$#,##0.00";
                worksheet.Cells[row, 4].Value = expense.CategoryId;
                worksheet.Cells[row, 5].Value = expense.PaymentMethodId;
                row++;
            }
            
            // Total
            worksheet.Cells[row, 2].Value = "TOTAL:";
            worksheet.Cells[row, 2].Style.Font.Bold = true;
            worksheet.Cells[row, 3].Formula = $"SUM(C2:C{row-1})";
            worksheet.Cells[row, 3].Style.Font.Bold = true;
            worksheet.Cells[row, 3].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[row, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 242, 204));
            
            // Ajustar columnas
            worksheet.Column(1).Width = 12;
            worksheet.Column(2).Width = 40;
            worksheet.Column(3).Width = 15;
            worksheet.Column(4).Width = 12;
            worksheet.Column(5).Width = 15;
            
            // Hoja 2: Resumen por categoría
            var summarySheet = package.Workbook.Worksheets.Add("Resumen por Categoría");
            
            summarySheet.Cells[1, 1].Value = "Categoría ID";
            summarySheet.Cells[1, 2].Value = "Total Gastado";
            summarySheet.Cells[1, 3].Value = "Cantidad";
            summarySheet.Cells[1, 4].Value = "Promedio";
            
            // Estilo de encabezados
            using (var range = summarySheet.Cells[1, 1, 1, 4])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
                range.Style.Font.Color.SetColor(Color.White);
            }
            
            // Agrupar por categoría
            var byCategory = expenses
                .GroupBy(e => e.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    Total = g.Sum(e => e.Amount),
                    Count = g.Count(),
                    Average = g.Average(e => e.Amount)
                })
                .OrderByDescending(x => x.Total)
                .ToList();
            
            row = 2;
            foreach (var category in byCategory)
            {
                summarySheet.Cells[row, 1].Value = category.CategoryId;
                summarySheet.Cells[row, 2].Value = category.Total;
                summarySheet.Cells[row, 2].Style.Numberformat.Format = "$#,##0.00";
                summarySheet.Cells[row, 3].Value = category.Count;
                summarySheet.Cells[row, 4].Value = category.Average;
                summarySheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00";
                row++;
            }
            
            summarySheet.Column(1).Width = 15;
            summarySheet.Column(2).Width = 15;
            summarySheet.Column(3).Width = 12;
            summarySheet.Column(4).Width = 15;
            
            return package.GetAsByteArray();
        }

        public async Task<object> MonthlyReport(int month, int year, int userId)
        {
            // Mes actual
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var currentMonthExpenses = await _expenseRepository.Filter(startDate, endDate, null, null, null, userId);

            // Mes anterior
            var previousMonth = startDate.AddMonths(-1);
            var previousStartDate = new DateTime(previousMonth.Year, previousMonth.Month, 1);
            var previousEndDate = previousStartDate.AddMonths(1).AddDays(-1);
            var previousMonthExpenses = await _expenseRepository.Filter(previousStartDate, previousEndDate, null, null, null, userId);

            // Totales
            var currentTotal = currentMonthExpenses.Sum(e => e.Amount);
            var previousTotal = previousMonthExpenses.Sum(e => e.Amount);
            var difference = currentTotal - previousTotal;
            var percentageChange = previousTotal > 0 ? ((difference / previousTotal) * 100) : 0;

            // Desglose por categoría del mes actual
            var byCategory = currentMonthExpenses
                .GroupBy(e => e.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    Total = g.Sum(e => e.Amount),
                    Count = g.Count(),
                    Average = g.Average(e => e.Amount)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            // Top 5 categorías
            var topCategories = byCategory.Take(5).ToList();

            // Comparación de cambios por categoría
            var categoryComparison = new List<object>();
            foreach (var current in byCategory)
            {
                var previousCategoryTotal = previousMonthExpenses
                    .Where(e => e.CategoryId == current.CategoryId)
                    .Sum(e => e.Amount);

                var categoryDifference = current.Total - previousCategoryTotal;
                var categoryChange = previousCategoryTotal > 0 
                    ? ((categoryDifference / previousCategoryTotal) * 100) 
                    : 0;

                categoryComparison.Add(new
                {
                    CategoryId = current.CategoryId,
                    CurrentTotal = current.Total,
                    PreviousTotal = previousCategoryTotal,
                    Difference = categoryDifference,
                    PercentageChange = Math.Round(categoryChange, 2)
                });
            }

            return new
            {
                Year = year,
                Month = month,
                MonthName = startDate.ToString("MMMM", new System.Globalization.CultureInfo("es-ES")),
                
                CurrentMonth = new
                {
                    TotalAmount = Math.Round(currentTotal, 2),
                    ExpenseCount = currentMonthExpenses.Count(),
                    AverageExpense = currentMonthExpenses.Any() 
                        ? Math.Round(currentMonthExpenses.Average(e => e.Amount), 2) 
                        : 0
                },

                PreviousMonth = new
                {
                    Year = previousMonth.Year,
                    Month = previousMonth.Month,
                    MonthName = previousMonth.ToString("MMMM", new System.Globalization.CultureInfo("es-ES")),
                    TotalAmount = Math.Round(previousTotal, 2),
                    ExpenseCount = previousMonthExpenses.Count()
                },

                Comparison = new
                {
                    Difference = Math.Round(difference, 2),
                    PercentageChange = Math.Round(percentageChange, 2),
                    Status = difference > 0 ? "AUMENTO" : difference < 0 ? "REDUCCIÓN" : "IGUAL"
                },

                ByCategory = byCategory,

                TopCategories = topCategories.Select((c, index) => new
                {
                    Rank = index + 1,
                    CategoryId = c.CategoryId,
                    Total = Math.Round(c.Total, 2),
                    Count = c.Count,
                    Percentage = Math.Round((c.Total / currentTotal) * 100, 2)
                }),

                CategoryComparison = categoryComparison
            };
        }
    }
}

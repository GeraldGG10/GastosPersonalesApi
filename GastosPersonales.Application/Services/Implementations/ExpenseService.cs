using System.Collections.Generic;
using System.Threading.Tasks;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Models;
using GastosPersonales.Application.DTOs;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Infrastructure.Repositories;
using OfficeOpenXml;
using System.Globalization;

namespace GastosPersonales.Application.Services.Implementations
{
    // Servicio para la gestión de gastos
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;

        // El Constructor que no puede faltar lider :)
        public ExpenseService(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        // Obtener todos los gastos
        public async Task<IEnumerable<Expense>> GetAll(int userId)
        {
            return await _expenseRepository.GetByUserId(userId);
        }

        // Obtener por su ID
        public async Task<Expense> GetById(int id, int userId)
        {
            var expense = await _expenseRepository.GetById(id, userId);
            if (expense == null) throw new KeyNotFoundException($"Expense with id {id} not found");
            return expense;
        }

        // Crear un nuevo gasto
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

        //Actualizar
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

        //Borrar
        public async Task<bool> Delete(int id, int userId)
        {
            return await _expenseRepository.Delete(id, userId);
        }

        // Importar desde Excel, la función más compleja del servicio, pero se logro :), eso quedo del kilo

        public async Task<ImportResultDTO> ImportFromExcel(Stream fileStream, string fileName, int userId)
        {
            var result = new ImportResultDTO();

            if (fileStream == null || fileStream.Length == 0)
            {
                result.Errors.Add("El archivo está vacío o no se recibió correctamente");
                return result;
            }

            if (!fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add("El archivo debe ser un Excel (.xlsx)");
                return result;
            }

            try
            {
                using var package = new ExcelPackage(fileStream);
                var worksheet = package.Workbook.Worksheets[0];
                
                if (worksheet == null)
                {
                    result.Errors.Add("El archivo Excel no contiene hojas");
                    return result;
                }

                var rowCount = worksheet.Dimension?.Rows ?? 0;
                
                if (rowCount < 2)
                {
                    result.Errors.Add("El archivo no contiene datos");
                    return result;
                }

                result.TotalRows = rowCount - 1;

                for (int row = 2; row <= rowCount; row++)
                {
                    var importedExpense = new ImportedExpenseDTO { Row = row };
                    var errors = new List<string>();

                    try
                    {
                        // Columna A: Fecha
                        var dateCell = worksheet.Cells[row, 1].Value;
                        if (dateCell == null)
                        {
                            errors.Add("Fecha es requerida");
                        }
                        else if (dateCell is DateTime dateValue)
                        {
                            importedExpense.Date = dateValue;
                        }
                        else if (DateTime.TryParse(dateCell.ToString(), out DateTime parsedDate))
                        {
                            importedExpense.Date = parsedDate;
                        }
                        else
                        {
                            errors.Add($"Fecha inválida: {dateCell}");
                        }

                        // Columna B: Descripción
                        var description = worksheet.Cells[row, 2].Value?.ToString() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(description))
                        {
                            errors.Add("Descripción es requerida");
                        }
                        importedExpense.Description = description;

                        // Columna C: Monto
                        var amountCell = worksheet.Cells[row, 3].Value;
                        if (amountCell == null)
                        {
                            errors.Add("Monto es requerido");
                        }
                        else
                        {
                            if (decimal.TryParse(amountCell.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
                            {
                                if (amount <= 0)
                                {
                                    errors.Add("El monto debe ser mayor a cero");
                                }
                                importedExpense.Amount = amount;
                            }
                            else
                            {
                                errors.Add($"Monto inválido: {amountCell}");
                            }
                        }

                        // Columna D: CategoryId
                        var categoryCell = worksheet.Cells[row, 4].Value;
                        if (categoryCell == null)
                        {
                            errors.Add("Categoría es requerida");
                        }
                        else if (int.TryParse(categoryCell.ToString(), out int categoryId))
                        {
                            if (categoryId <= 0)
                            {
                                errors.Add("ID de categoría debe ser mayor a cero");
                            }
                            importedExpense.CategoryId = categoryId;
                        }
                        else
                        {
                            errors.Add($"ID de categoría inválido: {categoryCell}");
                        }

                        // Columna E: PaymentMethodId
                        var paymentCell = worksheet.Cells[row, 5].Value;
                        if (paymentCell == null)
                        {
                            errors.Add("Método de pago es requerido");
                        }
                        else if (int.TryParse(paymentCell.ToString(), out int paymentMethodId))
                        {
                            if (paymentMethodId <= 0)
                            {
                                errors.Add("ID de método de pago debe ser mayor a cero");
                            }
                            importedExpense.PaymentMethodId = paymentMethodId;
                        }
                        else
                        {
                            errors.Add($"ID de método de pago inválido: {paymentCell}");
                        }

                        if (errors.Count > 0)
                        {
                            importedExpense.Status = "ERROR";
                            importedExpense.ErrorMessage = string.Join(", ", errors);
                            result.ErrorCount++;
                            result.Errors.Add($"Fila {row}: {importedExpense.ErrorMessage}");
                        }
                        else
                        {
                            var expense = new Expense
                            {
                                UserId = userId,
                                Date = importedExpense.Date,
                                Description = importedExpense.Description,
                                Amount = importedExpense.Amount,
                                CategoryId = importedExpense.CategoryId,
                                PaymentMethodId = importedExpense.PaymentMethodId
                            };

                            await _expenseRepository.Add(expense);
                            
                            importedExpense.Status = "INSERTADO";
                            result.SuccessCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        importedExpense.Status = "ERROR";
                        importedExpense.ErrorMessage = $"Error inesperado: {ex.Message}";
                        result.ErrorCount++;
                        result.Errors.Add($"Fila {row}: {importedExpense.ErrorMessage}");
                    }

                    result.ImportedExpenses.Add(importedExpense);
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Error al procesar el archivo: {ex.Message}");
            }

            return result;
        }
    }
}

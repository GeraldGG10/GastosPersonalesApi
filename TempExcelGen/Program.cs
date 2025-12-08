using OfficeOpenXml;
using System;
using System.IO;

namespace TempExcelGen
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var fileInfo = new FileInfo("../gastos_sample.xlsx");
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                using (var package = new ExcelPackage(fileInfo))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Gastos");

                    // Headers
                    worksheet.Cells[1, 1].Value = "Fecha";
                    worksheet.Cells[1, 2].Value = "Descripción";
                    worksheet.Cells[1, 3].Value = "Monto";
                    worksheet.Cells[1, 4].Value = "Categoría ID";
                    worksheet.Cells[1, 5].Value = "Método Pago ID";

                    // Sample Data
                    var data = new[]
                    {
                        new { Date = DateTime.Now, Desc = "Compra Supermercado", Amount = 150.50, CatId = 1, PayId = 1 },
                        new { Date = DateTime.Now.AddDays(-1), Desc = "Gasolina", Amount = 50.00, CatId = 2, PayId = 1 },
                        new { Date = DateTime.Now.AddDays(-2), Desc = "Cena Restaurante", Amount = 85.00, CatId = 1, PayId = 2 },
                        new { Date = DateTime.Now.AddDays(-3), Desc = "Pago Internet", Amount = 30.00, CatId = 3, PayId = 1 },
                        new { Date = DateTime.Now.AddDays(-5), Desc = "Café", Amount = 5.75, CatId = 1, PayId = 2 }
                    };

                    int row = 2;
                    foreach (var item in data)
                    {
                        worksheet.Cells[row, 1].Value = item.Date.ToString("yyyy-MM-dd");
                        worksheet.Cells[row, 2].Value = item.Desc;
                        worksheet.Cells[row, 3].Value = item.Amount;
                        worksheet.Cells[row, 4].Value = item.CatId;
                        worksheet.Cells[row, 5].Value = item.PayId;
                        row++;
                    }

                    package.Save();
                }
                Console.WriteLine("generated_success");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}

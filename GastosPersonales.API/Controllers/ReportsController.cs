using Microsoft.AspNetCore.Mvc;
using GastosPersonales.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GastosPersonales.API.Controllers
{
    // Controlador para la generación de reportes en formato TXT, JSON y Excel
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _service;

        public ReportsController(IReportService service)
        {
            _service = service;
        }

        // Filtrar gastos según varios criterios
        [HttpGet("filter")]
        public async Task<IActionResult> FilterExpenses(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int? categoryId,
            [FromQuery] int? paymentMethodId,
            [FromQuery] string? search)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);; // Obtenemos todo del token JWT
            var expenses = await _service.FilterExpenses(startDate, endDate, categoryId, paymentMethodId, search, userId);
            return Ok(expenses);
        }

        // Generar reporte mensual de gastos
        [HttpGet("monthly/{month}/{year}")]
        public async Task<IActionResult> MonthlyReport(int month, int year)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);; 
            var report = await _service.MonthlyReport(month, year, userId);
            return Ok(report);
        }
        // Exportar gastos en formato TXT
        [HttpGet("export/txt")]
        public async Task<IActionResult> ExportToTxt()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value); ; // Obtenemos todo del token JWT 
            var content = await _service.ExportExpensesToTxt(userId);
            return File(System.Text.Encoding.UTF8.GetBytes(content), "text/plain", $"gastos_{DateTime.Now:yyyyMMdd}.txt");
        }

        // Exportar formato JSON
        [HttpGet("export/json")]
        public async Task<IActionResult> ExportToJson()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);; 
            var content = await _service.ExportExpensesToJson(userId);
            return File(System.Text.Encoding.UTF8.GetBytes(content), "application/json", $"gastos_{DateTime.Now:yyyyMMdd}.json");
        }

        // Exportar formato Excel
        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportToExcel()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);; 
            var excelBytes = await _service.ExportExpensesToExcel(userId);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"gastos_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}





using Microsoft.AspNetCore.Mvc;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Models;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route(""api/[controller]"")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _service;

        public ReportsController(IReportService service)
        {
            _service = service;
        }

        [HttpGet(""filter"")]
        public async Task<IActionResult> Filter(DateTime? startDate, DateTime? endDate, int? categoryId, int? paymentMethodId, string? search)
        {
            var result = await _service.FilterExpenses(startDate, endDate, categoryId, paymentMethodId, search, 1); // userId dummy
            return Ok(result);
        }

        [HttpGet(""monthly"")]
        public async Task<IActionResult> MonthlyReport(int month, int year)
        {
            var report = await _service.MonthlyReport(month, year, 1);
            return Ok(report);
        }

        [HttpGet(""export/txt"")]
        public async Task<IActionResult> ExportTxt()
        {
            var txt = await _service.ExportExpensesToTxt(1);
            return Ok(txt);
        }

        [HttpGet(""export/json"")]
        public async Task<IActionResult> ExportJson()
        {
            var json = await _service.ExportExpensesToJson(1);
            return Ok(json);
        }
    }
}

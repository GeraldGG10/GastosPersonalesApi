using Microsoft.AspNetCore.Mvc;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Models;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetService _service;

        public BudgetsController(IBudgetService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var budgets = await _service.GetAll(1); // userId dummy
            return Ok(budgets);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BudgetDTO dto)
        {
            var budget = await _service.Create(dto, 1);
            return Ok(budget);
        }

        [HttpGet("percentage")]
        public async Task<IActionResult> GetPercentage(int categoryId, int month, int year)
        {
            var percent = await _service.CalculateSpentPercentage(categoryId, month, year, 1);
            return Ok(new { Percentage = percent });
        }
    }
}

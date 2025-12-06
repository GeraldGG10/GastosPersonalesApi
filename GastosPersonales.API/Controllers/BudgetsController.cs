using Microsoft.AspNetCore.Mvc;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Models;
using Microsoft.AspNetCore.Authorization;

namespace GastosPersonales.API.Controllers
{
    [Authorize]
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
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);; // TODO: Obtener del token JWT
            var budgets = await _service.GetAll(userId);
            return Ok(budgets);
        }

        [HttpGet("{categoryId}/{month}/{year}")]
        public async Task<IActionResult> GetByCategoryMonth(int categoryId, int month, int year)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);; // TODO: Obtener del token JWT
            try
            {
                var budget = await _service.GetByCategoryMonth(categoryId, month, year, userId);
                return Ok(budget);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Presupuesto no encontrado" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BudgetDTO dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);; // TODO: Obtener del token JWT
            var budget = await _service.Create(dto, userId);
            return CreatedAtAction(nameof(GetByCategoryMonth), 
                new { categoryId = budget.CategoryId, month = budget.Month, year = budget.Year }, 
                budget);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BudgetDTO dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);; // TODO: Obtener del token JWT
            try
            {
                var budget = await _service.Update(id, dto, userId);
                return Ok(budget);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Presupuesto no encontrado" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);; // TODO: Obtener del token JWT
            var success = await _service.Delete(id, userId);
            if (!success) return NotFound(new { Message = "Presupuesto no encontrado" });
            return NoContent();
        }

        [HttpGet("percentage/{categoryId}/{month}/{year}")]
        public async Task<IActionResult> GetSpentPercentage(int categoryId, int month, int year)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);; // TODO: Obtener del token JWT
            var percentage = await _service.CalculateSpentPercentage(categoryId, month, year, userId);
            return Ok(new { CategoryId = categoryId, Month = month, Year = year, Percentage = percentage });
        }

        [HttpGet("exceeded/{month}/{year}")]
        public async Task<IActionResult> GetExceeded(int month, int year)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);; // TODO: Obtener del token JWT
            var exceeded = await _service.GetExceededBudgets(month, year, userId);
            return Ok(exceeded);
        }
    }
}





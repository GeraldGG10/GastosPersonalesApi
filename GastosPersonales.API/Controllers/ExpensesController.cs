using Microsoft.AspNetCore.Mvc;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Models;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route(""api/[controller]"")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _service;

        public ExpensesController(IExpenseService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var expenses = await _service.GetAll(1); // userId dummy
            return Ok(expenses);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExpenseDTO dto)
        {
            var expense = await _service.Create(dto, 1); // userId dummy
            return Ok(expense);
        }
    }
}

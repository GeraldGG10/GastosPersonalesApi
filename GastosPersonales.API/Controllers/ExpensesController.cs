
using Microsoft.AspNetCore.Mvc;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;

namespace GastosPersonales.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _service;

        public ExpensesController(IExpenseService service)
        {
            _service = service;
        }

        // GET: api/expenses
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var expenses = await _service.GetAll(userId);
            return Ok(expenses);
        }

        // GET: api/expenses/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();
            try
            {
                var expense = await _service.GetById(id, userId);
                return Ok(expense);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Gasto no encontrado" });
            }
        }

        // POST: api/expenses
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExpenseDTO dto)
        {
            var userId = GetUserId();
            var expense = await _service.Create(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
        }

        // PUT: api/expenses/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExpenseDTO dto)
        {
            var userId = GetUserId();
            try
            {
                var expense = await _service.Update(id, dto, userId);
                return Ok(expense);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Gasto no encontrado" });
            }
        }

        // DELETE: api/expenses/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var success = await _service.Delete(id, userId);
            if (!success) return NotFound(new { Message = "Gasto no encontrado" });
            return NoContent();
        }

        // POST: api/expenses/import
        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportFromExcel([FromForm] ExpenseUploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Archivo no proporcionado.");

            var userId = GetUserId();

            using var stream = new MemoryStream();
            await request.File.CopyToAsync(stream);
            stream.Position = 0;

            var result = await _service.ImportFromExcel(stream, request.File.FileName, userId);

            if (result.ErrorCount > 0 && result.SuccessCount == 0)
                return BadRequest(result);

            return Ok(result);
        }

        // DTO para Swagger y multipart/form-data
        public class ExpenseUploadRequest
        {
            public IFormFile File { get; set; }
        }

        // Método helper para obtener el Id del usuario
        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
    }
}

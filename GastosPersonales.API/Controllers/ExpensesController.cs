using Microsoft.AspNetCore.Mvc;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var expenses = await _service.GetAll(userId);
            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExpenseDTO dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var expense = await _service.Create(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExpenseDTO dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var success = await _service.Delete(id, userId);
            if (!success) return NotFound(new { Message = "Gasto no encontrado" });
            return NoContent();
        }

        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        [RequestFormLimits(MultipartBodyLengthLimit = 10485760)] // 10MB
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ImportFromExcel([FromForm] IFormFile file)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Debe proporcionar un archivo" });
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            var result = await _service.ImportFromExcel(stream, file.FileName, userId);

            if (result.ErrorCount > 0 && result.SuccessCount == 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}







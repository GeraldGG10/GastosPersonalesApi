using Microsoft.AspNetCore.Mvc;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Models;
using Microsoft.AspNetCore.Authorization;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var categories = await _service.GetAll(userId);
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var category = await _service.Create(dto, userId);
            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDTO dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var category = await _service.Update(id, dto, userId);
            if (category == null) return NotFound(new { Message = "Categoría no encontrada" });
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var success = await _service.Delete(id, userId);
            if (!success) return NotFound(new { Message = "Categoría no encontrada" });
            return NoContent();
        }
    }
}




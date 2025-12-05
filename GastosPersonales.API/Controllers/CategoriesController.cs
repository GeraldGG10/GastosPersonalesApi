using Microsoft.AspNetCore.Mvc;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Models;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route(""api/[controller]"")]
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
            // Para pruebas, usamos userId = 1
            var categories = await _service.GetAll(1);
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO dto)
        {
            var category = await _service.Create(dto, 1);
            return Ok(category);
        }
    }
}

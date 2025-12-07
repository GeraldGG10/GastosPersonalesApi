using Microsoft.AspNetCore.Mvc;
using GastosPersonales.Application.Models;
using GastosPersonales.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IMethodService _service;

        public PaymentMethodsController(IMethodService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var methods = await _service.GetAll(userId);
            return Ok(methods);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaymentMethodDTO dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var method = await _service.Create(dto, userId);
            return Ok(method);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var success = await _service.Delete(id, userId);
            if (!success) return NotFound(new { Message = "Método de pago no encontrado" });
            return NoContent();
        }
    }
}








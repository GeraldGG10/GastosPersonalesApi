using Microsoft.AspNetCore.Mvc;
using GastosPersonales.Application.Models;
using GastosPersonales.Application.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Models;
using Microsoft.AspNetCore.Authorization;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentMethodsController : ControllerBase // ✅ Corregido
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
    }
}








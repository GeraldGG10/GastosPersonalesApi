using Microsoft.AspNetCore.Mvc;
using GastosPersonales.Application.Models;
using GastosPersonales.Application.Services.Interfaces;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route(""api/[controller]"")]
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
            var methods = await _service.GetAll(1); // userId dummy
            return Ok(methods);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaymentMethodDTO dto)
        {
            var method = await _service.Create(dto, 1);
            return Ok(method);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SaludController : ControllerBase // ✅ Corregido
    {
        [HttpGet]
        public IActionResult Get() => Ok(new { status = "OK", timestamp = System.DateTime.UtcNow });
    }
}







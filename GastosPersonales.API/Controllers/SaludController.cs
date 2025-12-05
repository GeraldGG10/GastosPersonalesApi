using Microsoft.AspNetCore.Mvc;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaludController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new { status = "OK", timestamp = System.DateTime.UtcNow });
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GastosPersonales.API.Controllers
{
    // Controlador de salud para verificar el estado de mi API
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SaludController : ControllerBase 
    {
        [HttpGet]
        public IActionResult Get() => Ok(new { status = "OK", timestamp = System.DateTime.UtcNow });
    }
}







# modulo-B.ps1
param()
$ErrorActionPreference = "Stop"
cd (Get-Location)

Write-Host "Módulo B: CRUD usuarios, categorias, metodos de pago, gastos..."

# Paquetes ya deben existir; añadimos AutoMapper DTO mapping helper if not present
dotnet add .\GastosPersonales.Application\GastosPersonales.Application.csproj package AutoMapper --version 12.0.1

# Create DTOs folders
New-Item -ItemType Directory -Path ".\GastosPersonales.Application\DTOs/Categorias" -Force | Out-Null
New-Item -ItemType Directory -Path ".\GastosPersonales.Application\DTOs/MetodosPago" -Force | Out-Null
New-Item -ItemType Directory -Path ".\GastosPersonales.Application\DTOs/Gastos" -Force | Out-Null

# Sample DTOs
@'
namespace GastosPersonales.Application.DTOs.Categorias
{
    public record CategoriaCreateDto(string Nombre);
    public record CategoriaDto(int Id, string Nombre, int UsuarioId, bool Activo);
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.Application\DTOs/Categorias/CategoriaDtos.cs

@'
namespace GastosPersonales.Application.DTOs.MetodosPago
{
    public record MetodoPagoCreateDto(string Nombre);
    public record MetodoPagoDto(int Id, string Nombre, int UsuarioId);
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.Application\DTOs/MetodosPago/MetodoPagoDtos.cs

@'
namespace GastosPersonales.Application.DTOs.Gastos
{
    public record GastoCreateDto(decimal Monto, System.DateTime Fecha, int CategoriaId, int MetodoPagoId, string? Descripcion);
    public record GastoDto(int Id, decimal Monto, System.DateTime Fecha, int CategoriaId, int MetodoPagoId, string? Descripcion, int UsuarioId);
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.Application\DTOs/Gastos/GastoDtos.cs

# Create Controllers in API (UsuariosController, CategoriasController, MetodosController, GastosController)
New-Item -ItemType Directory -Path ".\GastosPersonales.API\Controllers" -Force | Out-Null

@'
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GastosPersonales.Domain.Interfaces;
using GastosPersonales.Application.DTOs.Categorias;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepositorio _repo;
        public CategoriasController(ICategoriaRepositorio repo) { _repo = repo; }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoriaCreateDto dto)
        {
            var entidad = new GastosPersonales.Domain.Entities.Categoria { Nombre = dto.Nombre, UsuarioId = int.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? "0"),  };
            await _repo.AgregarAsync(entidad);
            return CreatedAtAction(nameof(GetByUser), new { id = entidad.Id }, entidad);
        }

        [HttpGet]
        public async Task<IActionResult> GetByUser()
        {
            var usuarioId = int.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? "0");
            var lista = await _repo.ObtenerPorUsuarioIdAsync(usuarioId);
            return Ok(lista);
        }
    }
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.API\Controllers\CategoriasController.cs

@'
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GastosPersonales.Domain.Interfaces;
using GastosPersonales.Application.DTOs.MetodosPago;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MetodosPagoController : ControllerBase
    {
        private readonly IMetodoPagoRepositorio _repo;
        public MetodosPagoController(IMetodoPagoRepositorio repo) { _repo = repo; }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MetodoPagoCreateDto dto)
        {
            var entidad = new GastosPersonales.Domain.Entities.MetodoPago { Nombre = dto.Nombre, UsuarioId = int.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? "0") };
            await _repo.AgregarAsync(entidad);
            return CreatedAtAction(nameof(GetByUser), new { id = entidad.Id }, entidad);
        }

        [HttpGet]
        public async Task<IActionResult> GetByUser()
        {
            var usuarioId = int.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? "0");
            var lista = await _repo.ObtenerPorUsuarioIdAsync(usuarioId);
            return Ok(lista);
        }
    }
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.API\Controllers\MetodosPagoController.cs

@'
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GastosPersonales.Domain.Interfaces;
using GastosPersonales.Application.DTOs.Gastos;
using GastosPersonales.Domain.Entities;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GastosController : ControllerBase
    {
        private readonly IGastoRepositorio _repo;
        public GastosController(IGastoRepositorio repo) { _repo = repo; }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GastoCreateDto dto)
        {
            if (dto.Monto <= 0) return BadRequest("Monto debe ser positivo");
            var usuarioId = int.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? "0");
            var entidad = new Gasto { Monto = dto.Monto, Fecha = dto.Fecha, CategoriaId = dto.CategoriaId, MetodoPagoId = dto.MetodoPagoId, Descripcion = dto.Descripcion, UsuarioId = usuarioId };
            await _repo.AgregarAsync(entidad);
            return CreatedAtAction(nameof(GetByUser), new { id = entidad.Id }, entidad);
        }

        [HttpGet]
        public async Task<IActionResult> GetByUser()
        {
            var usuarioId = int.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? "0");
            var lista = await _repo.ObtenerPorUsuarioIdAsync(usuarioId);
            return Ok(lista);
        }
    }
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.API\Controllers\GastosController.cs

dotnet restore
dotnet build
Write-Host "Módulo B completado."

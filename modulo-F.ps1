# modulo-F.ps1
param()
$ErrorActionPreference = "Stop"
cd (Get-Location)

Write-Host "Módulo F: Presupuestos por categoría..."

# Add entity in Domain
@'
namespace GastosPersonales.Domain.Entities
{
    public class Presupuesto : BaseEntity
    {
        public int CategoriaId { get; set; }
        public int UsuarioId { get; set; }
        public decimal Monto { get; set; }
        public int Mes { get; set; } // 1..12
        public int Year { get; set; }
    }
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.Domain\Entities\Presupuesto.cs

# Add DbSet in AplicacionDbContext
$ctx = Get-Content .\GastosPersonales.Infrastructure\Persistencia\AplicacionDbContext.cs
if (-not ($ctx -match "DbSet<Presupuesto>")) {
    $ctx = $ctx -replace "public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();", "public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();`n        public DbSet<Presupuesto> Presupuestos => Set<Presupuesto>();"
    $ctx | Set-Content .\GastosPersonales.Infrastructure\Persistencia\AplicacionDbContext.cs
}

# Repository interface in Domain (append)
@'
namespace GastosPersonales.Domain.Interfaces
{
    public interface IPresupuestoRepositorio
    {
        Task AgregarAsync(GastosPersonales.Domain.Entities.Presupuesto p);
        Task<List<GastosPersonales.Domain.Entities.Presupuesto>> ObtenerPorUsuarioAsync(int usuarioId);
    }
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.Domain\Interfaces\IPresupuestoRepositorio.cs

# Repository implementation in Infrastructure
@'
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Interfaces;
using GastosPersonales.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace GastosPersonales.Infrastructure.Repositorios
{
    public class PresupuestoRepositorio : IPresupuestoRepositorio
    {
        private readonly AplicacionDbContext _db;
        public PresupuestoRepositorio(AplicacionDbContext db) { _db = db; }

        public async Task AgregarAsync(Presupuesto p) { _db.Presupuestos.Add(p); await _db.SaveChangesAsync(); }
        public async Task<List<Presupuesto>> ObtenerPorUsuarioAsync(int usuarioId) => await _db.Presupuestos.Where(x => x.UsuarioId == usuarioId).ToListAsync();
    }
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.Infrastructure\Repositorios\PresupuestoRepositorio.cs

# Controller for Presupuestos
@'
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GastosPersonales.Domain.Interfaces;
using GastosPersonales.Domain.Entities;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PresupuestosController : ControllerBase
    {
        private readonly IPresupuestoRepositorio _repo;
        private readonly AplicacionDbContext _db;
        public PresupuestosController(IPresupuestoRepositorio repo, AplicacionDbContext db) { _repo = repo; _db = db; }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Presupuesto p)
        {
            p.UsuarioId = int.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? "0");
            await _repo.AgregarAsync(p);
            return Ok(p);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var usuarioId = int.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? "0");
            var lista = await _repo.ObtenerPorUsuarioAsync(usuarioId);
            // compute alerts (50,80,100) per category
            var res = new List<object>();
            foreach(var pr in lista)
            {
                var gastos = await _db.Gastos.Where(g => g.UsuarioId == usuarioId && g.CategoriaId == pr.CategoriaId && g.Fecha.Year == pr.Year && g.Fecha.Month == pr.Mes).ToListAsync();
                var total = gastos.Sum(g => g.Monto);
                var percent = pr.Monto == 0 ? 0 : (double)(total / pr.Monto * 100);
                res.Add(new { Presupuesto = pr, Gastado = total, Porcentaje = percent });
            }
            return Ok(res);
        }
    }
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.API\Controllers\PresupuestosController.cs

# Register repo in DI (Program.cs)
$prog = Get-Content .\GastosPersonales.API\Program.cs
if (-not ($prog -match "PresupuestoRepositorio")) {
    $prog = $prog -replace "builder.Services.AddScoped<IGeneradorJwt, GeneradorJwt>();", "builder.Services.AddScoped<IGeneradorJwt, GeneradorJwt>();`nbuilder.Services.AddScoped<IPresupuestoRepositorio, PresupuestoRepositorio>();"
    $prog | Set-Content .\GastosPersonales.API\Program.cs
}

dotnet restore
dotnet build
Write-Host "Módulo F completado."

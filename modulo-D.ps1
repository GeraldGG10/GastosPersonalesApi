# modulo-D.ps1
param()
$ErrorActionPreference = "Stop"
cd (Get-Location)

Write-Host "Módulo D: Reportes y filtros..."

# Crear servicio de reportes en Application + interfaz y su implementación en Infrastructure
New-Item -ItemType Directory -Path ".\GastosPersonales.Application\Services\Reports" -Force | Out-Null
@'
using System.Collections.Generic;
using System.Threading.Tasks;
using GastosPersonales.Application.DTOs.Gastos;

namespace GastosPersonales.Application.Services.Interfaces
{
    public interface IReporteService
    {
        Task<object> ReporteMensual(int usuarioId, int year, int month);
        Task<object> TopCategorias(int usuarioId, int year, int month, int topN);
    }
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.Application\Services\Interfaces\IReporteService.cs

# Implementation in Infrastructure (uses EF context)
@'
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GastosPersonales.Infrastructure.Repositorios
{
    public class ReporteService : IReporteService
    {
        private readonly AplicacionDbContext _db;
        public ReporteService(AplicacionDbContext db) { _db = db; }

        public async Task<object> ReporteMensual(int usuarioId, int year, int month)
        {
            var gastos = await _db.Gastos.Where(g => g.UsuarioId == usuarioId && g.Fecha.Year == year && g.Fecha.Month == month).ToListAsync();
            var total = gastos.Sum(g => g.Monto);
            var byCategory = gastos.GroupBy(g => g.CategoriaId).Select(g => new { CategoriaId = g.Key, Total = g.Sum(x => x.Monto) }).ToList();
            return new { Total = total, PorCategoria = byCategory };
        }

        public async Task<object> TopCategorias(int usuarioId, int year, int month, int topN)
        {
            var gastos = await _db.Gastos.Where(g => g.UsuarioId == usuarioId && g.Fecha.Year == year && g.Fecha.Month == month)
                                         .GroupBy(x => x.CategoriaId)
                                         .Select(g => new { CategoriaId = g.Key, Total = g.Sum(x => x.Monto) })
                                         .OrderByDescending(x => x.Total)
                                         .Take(topN)
                                         .ToListAsync();
            return gastos;
        }
    }
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.Infrastructure\Repositorios\ReporteService.cs

# Controller endpoints
@'
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GastosPersonales.Application.Services.Interfaces;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reportes;
        public ReportesController(IReporteService reportes) { _reportes = reportes; }

        [HttpGet("mensual")]
        public async Task<IActionResult> Mensual(int year, int month)
        {
            var usuarioId = int.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? "0");
            var res = await _reportes.ReporteMensual(usuarioId, year, month);
            return Ok(res);
        }

        [HttpGet("top")]
        public async Task<IActionResult> Top(int year, int month, int n = 5)
        {
            var usuarioId = int.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? "0");
            var res = await _reportes.TopCategorias(usuarioId, year, month, n);
            return Ok(res);
        }
    }
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.API\Controllers\ReportesController.cs

# Register service in DI (Program.cs)
$prog = Get-Content .\GastosPersonales.API\Program.cs
if (-not ($prog -match "ReporteService")) {
    $prog = $prog -replace "builder.Services.AddScoped<IGeneradorJwt, GeneradorJwt>();", "builder.Services.AddScoped<IGeneradorJwt, GeneradorJwt>();`nbuilder.Services.AddScoped<GastosPersonales.Application.Services.Interfaces.IReporteService, GastosPersonales.Infrastructure.Repositorios.ReporteService>();"
    $prog | Set-Content .\GastosPersonales.API\Program.cs
}

dotnet restore
dotnet build
Write-Host "Módulo D completado."

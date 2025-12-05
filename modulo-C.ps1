# modulo-C.ps1
param()
$ErrorActionPreference = "Stop"
cd (Get-Location)

Write-Host "Módulo C: Importar desde Excel (ClosedXML)..."

# Añadir ClosedXML
dotnet add .\GastosPersonales.API\GastosPersonales.API.csproj package ClosedXML --version 1.0.0
dotnet add .\GastosPersonales.Infrastructure\GastosPersonales.Infrastructure.csproj package ClosedXML --version 1.0.0

# Controller endpoint to upload file
@'
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Interfaces;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ImportController : ControllerBase
    {
        private readonly IGastoRepositorio _gastoRepo;
        public ImportController(IGastoRepositorio gastoRepo) { _gastoRepo = gastoRepo; }

        [HttpPost("excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("Archivo inválido");
            var report = new List<object>();
            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var ws = workbook.Worksheets.First();
            var rows = ws.RowsUsed().Skip(1); // assume header
            foreach (var r in rows)
            {
                try
                {
                    var monto = decimal.Parse(r.Cell(1).GetString());
                    var fecha = DateTime.Parse(r.Cell(2).GetString());
                    var categoriaId = int.Parse(r.Cell(3).GetString());
                    var metodoId = int.Parse(r.Cell(4).GetString());
                    var descripcion = r.Cell(5).GetString();
                    if (monto <= 0) throw new Exception("Monto inválido");
                    var usuarioId = int.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? "0");
                    var gasto = new Gasto { Monto = monto, Fecha = fecha, CategoriaId = categoriaId, MetodoPagoId = metodoId, Descripcion = descripcion, UsuarioId = usuarioId };
                    await _gastoRepo.AgregarAsync(gasto);
                    report.Add(new { Row = r.RowNumber(), Status = "OK" });
                }
                catch (Exception ex)
                {
                    report.Add(new { Row = r.RowNumber(), Status = "ERROR", Message = ex.Message });
                }
            }
            return Ok(report);
        }
    }
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.API\Controllers\ImportController.cs

dotnet restore
dotnet build
Write-Host "Módulo C completado."

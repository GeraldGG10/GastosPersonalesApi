# modulo-E.ps1
param()
$ErrorActionPreference = "Stop"
cd (Get-Location)

Write-Host "Módulo E: Exportaciones (Excel via ClosedXML, TXT, JSON)..."

# Ensure ClosedXML is installed (API)
dotnet add .\GastosPersonales.API\GastosPersonales.API.csproj package ClosedXML --version 1.0.0

# Controller for exports
@'
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using GastosPersonales.Infrastructure.Persistencia;
using System.Text.Json;

namespace GastosPersonales.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExportController : ControllerBase
    {
        private readonly AplicacionDbContext _db;
        public ExportController(AplicacionDbContext db) { _db = db; }

        [HttpGet("excel")]
        public IActionResult ExportExcel()
        {
            var usuarioId = int.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? "0");
            var gastos = _db.Gastos.Where(g => g.UsuarioId == usuarioId).ToList();
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Gastos");
            ws.Cell(1,1).Value = "Monto";
            ws.Cell(1,2).Value = "Fecha";
            ws.Cell(1,3).Value = "CategoriaId";
            ws.Cell(1,4).Value = "MetodoPagoId";
            ws.Cell(1,5).Value = "Descripcion";
            int r = 2;
            foreach(var g in gastos) {
                ws.Cell(r,1).Value = g.Monto;
                ws.Cell(r,2).Value = g.Fecha;
                ws.Cell(r,3).Value = g.CategoriaId;
                ws.Cell(r,4).Value = g.MetodoPagoId;
                ws.Cell(r,5).Value = g.Descripcion;
                r++;
            }
            using var ms = new System.IO.MemoryStream();
            wb.SaveAs(ms);
            ms.Seek(0,0);
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "gastos.xlsx");
        }

        [HttpGet("json")]
        public IActionResult ExportJson()
        {
            var usuarioId = int.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? "0");
            var gastos = _db.Gastos.Where(g => g.UsuarioId == usuarioId).ToList();
            return File(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(gastos)), "application/json", "gastos.json");
        }

        [HttpGet("txt")]
        public IActionResult ExportTxt()
        {
            var usuarioId = int.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? "0");
            var gastos = _db.Gastos.Where(g => g.UsuarioId == usuarioId).ToList();
            var sb = new System.Text.StringBuilder();
            foreach(var g in gastos) sb.AppendLine($"{g.Fecha:yyyy-MM-dd}\t{g.Monto}\t{g.Descripcion}");
            return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()), "text/plain", "gastos.txt");
        }
    }
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.API\Controllers\ExportController.cs

dotnet restore
dotnet build
Write-Host "Módulo E completado."

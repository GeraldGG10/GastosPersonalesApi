# modulo-G.ps1
param()
$ErrorActionPreference = "Stop"
cd (Get-Location)

Write-Host "Módulo G: Middleware global de excepciones y Serilog..."

# Add Serilog packages
dotnet add .\GastosPersonales.API\GastosPersonales.API.csproj package Serilog.AspNetCore --version 7.0.0
dotnet add .\GastosPersonales.API\GastosPersonales.API.csproj package Serilog.Sinks.Console --version 4.0.1

# Add middleware file
@'
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using Serilog;

namespace GastosPersonales.API.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlerMiddleware(RequestDelegate next) { _next = next; }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var res = JsonSerializer.Serialize(new { message = ex?.Message });
                await context.Response.WriteAsync(res);
            }
        }
    }

    public static class ErrorHandlerExtensions
    {
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder app) => app.UseMiddleware<ErrorHandlerMiddleware>();
    }
}
'@ | Out-File -Encoding utf8 .\GastosPersonales.API\Middleware\ErrorHandlerMiddleware.cs

# Update Program.cs to use Serilog and middleware (prepend using Serilog)
$prog = Get-Content .\GastosPersonales.API\Program.cs
if (-not ($prog -match "Serilog")) {
    $prog = $prog -replace "var builder = WebApplication.CreateBuilder\\(args\\);", "using Serilog;`nvar builder = WebApplication.CreateBuilder(args);"
    $prog = $prog -replace "var app = builder.Build();", "Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();`nvar app = builder.Build();"
    $prog = $prog -replace "if \\(app.Environment.IsDevelopment\\(\\)\\)\\n\\{", "app.UseErrorHandler();`nif (app.Environment.IsDevelopment()) {"
    $prog | Set-Content .\GastosPersonales.API\Program.cs
}

dotnet restore
dotnet build
Write-Host "Módulo G completado."

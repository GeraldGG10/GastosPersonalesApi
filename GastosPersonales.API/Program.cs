using GastosPersonales.Application.Services.Implementations;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Domain.Interfaces;
using GastosPersonales.Infrastructure.Autenticacion;
using GastosPersonales.Infrastructure.Persistencia;
using GastosPersonales.Infrastructure.Repositories;
using GastosPersonales.Infrastructure.Repositorios;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// 🔵 CONFIGURAR CORS
// -----------------------------
var MyCors = "_myCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyCors, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// -----------------------------
// 🔵 BASE DE DATOS
// -----------------------------
builder.Services.AddDbContext<AplicacionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=(localdb)\\mssqllocaldb;Database=GastosDB;Trusted_Connection=True;"));

// -----------------------------
// 🔵 INYECCIÓN DE DEPENDENCIAS
// -----------------------------
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IMetodoPagoRepositorio, MetodoPagoRepositorio>();
builder.Services.AddScoped<IGastoRepositorio, GastoRepositorio>();
builder.Services.AddScoped<IGeneradorJwt, GeneradorJwt>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();

// -----------------------------
// 🔵 AUTENTICACIÓN JWT
// -----------------------------
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = false
        };
    });

// -----------------------------
// 🔵 SWAGGER
// -----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GastosPersonales API",
        Version = "v1"
    });

    // Mapear IFormFile correctamente para Swagger
    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
});

// -----------------------------
// 🔵 AUTOMAPPER
// -----------------------------
builder.Services.AddAutoMapper(typeof(Program));

// -----------------------------
// 🔵 CONTROLLERS
// -----------------------------
builder.Services.AddControllers();

// -----------------------------
// 🔵 APP
// -----------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GastosPersonales API v1");
    });
}

// -----------------------------
// 🔵 MIDDLEWARES
// -----------------------------
app.UseCors(MyCors);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

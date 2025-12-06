using GastosPersonales.Infrastructure.Persistencia;
using GastosPersonales.Infrastructure.Repositorios;
using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Services.Implementations;
using GastosPersonales.Domain.Interfaces;
using GastosPersonales.Infrastructure.Autenticacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------
// 🔵 CONFIGURAR CORS PARA FRONTEND
// --------------------------------------
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

// --------------------------------------
// 🔵 BASE DE DATOS
// --------------------------------------
builder.Services.AddDbContext<AplicacionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=(localdb)\\mssqllocaldb;Database=GastosDB;Trusted_Connection=True;"));

// --------------------------------------
// 🔵 INYECCIÓN DE DEPENDENCIAS
// --------------------------------------
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IMetodoPagoRepositorio, MetodoPagoRepositorio>();
builder.Services.AddScoped<IGastoRepositorio, GastoRepositorio>();
builder.Services.AddScoped<IGeneradorJwt, GeneradorJwt>();

// Application Services
builder.Services.AddScoped<GastosPersonales.Application.Services.Interfaces.IAuthService, GastosPersonales.Application.Services.Implementations.AuthService>();
builder.Services.AddScoped<GastosPersonales.Infrastructure.Repositories.IExpenseRepository, GastosPersonales.Infrastructure.Repositories.ExpenseRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();

// --------------------------------------
// 🔵 AUTENTICACIÓN JWT
// --------------------------------------
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = false // ⚠️ Cambiar cuando agregues KEY real
        };
    });

// --------------------------------------
// 🔵 SWAGGER
// --------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GastosPersonales API",
        Version = "v1"
    });
});

// --------------------------------------
// 🔵 AUTOMAPPER
// --------------------------------------
builder.Services.AddAutoMapper(typeof(Program));

// --------------------------------------
// 🔵 CONTROLLERS
// --------------------------------------
builder.Services.AddControllers();

var app = builder.Build();

// --------------------------------------
// 🔵 ENTORNO DESARROLLO
// --------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --------------------------------------
// 🔵 ORDEN CORRECTO DE MIDDLEWARES
// --------------------------------------
app.UseCors(MyCors);            // ← DEBE IR ANTES DE AUTH
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();


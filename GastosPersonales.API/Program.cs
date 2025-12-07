using GastosPersonales.Application.Services.Interfaces;
using GastosPersonales.Application.Services.Implementations;
using GastosPersonales.Infrastructure.Autenticacion;
using GastosPersonales.Infrastructure.Persistencia;
using GastosPersonales.Infrastructure.Repositories;
using GastosPersonales.Infrastructure.Repositorios;
using GastosPersonales.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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
// 🔵 BASE DE DATOS - SQLITE ✅
// -----------------------------
builder.Services.AddDbContext<AplicacionDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=GastosDB.db"));

// -----------------------------
// 🔵 INYECCIÓN DE DEPENDENCIAS - REPOSITORIOS
// -----------------------------
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IMetodoPagoRepositorio, MetodoPagoRepositorio>();
builder.Services.AddScoped<IGastoRepositorio, GastoRepositorio>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IGeneradorJwt, GeneradorJwt>();

// -----------------------------
// 🔵 INYECCIÓN DE DEPENDENCIAS - SERVICIOS ✅
// -----------------------------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IMethodService, MethodService>();

// -----------------------------
// 🔵 AUTENTICACIÓN JWT
// -----------------------------
var jwtKey = builder.Configuration["Jwt:Key"] ?? "tu-clave-secreta-super-segura-de-al-menos-32-caracteres-para-jwt";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "GastosPersonalesAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "GastosPersonalesFrontend";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// -----------------------------
// 🔵 SWAGGER
// -----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GastosPersonales API",
        Version = "v1",
        Description = "API para gestión de gastos personales con autenticación JWT"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
});

// -----------------------------
// 🔵 AUTOMAPPER (si lo usas)
// -----------------------------
// builder.Services.AddAutoMapper(typeof(Program));

// -----------------------------
// 🔵 CONTROLLERS
// -----------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// -----------------------------
// 🔵 BUILD APP
// -----------------------------
var app = builder.Build();

// ✅ CREAR BASE DE DATOS AUTOMÁTICAMENTE AL INICIAR
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AplicacionDbContext>();
    try
    {
        db.Database.EnsureCreated(); // Crea la BD si no existe
        Console.WriteLine("✅ Base de datos creada/verificada correctamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al crear la base de datos: {ex.Message}");
    }
}

// -----------------------------
// 🔵 CONFIGURACIÓN DE DESARROLLO
// -----------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GastosPersonales API v1");
        // c.RoutePrefix = string.Empty; // Descomenta para poner Swagger en la raíz
    });
}

// -----------------------------
// 🔵 MIDDLEWARES
// -----------------------------
app.UseHttpsRedirection();
app.UseCors(MyCors);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("🚀 API de Gastos Personales iniciada correctamente");
Console.WriteLine($"📊 Swagger UI disponible en: https://localhost:7262");

app.Run();
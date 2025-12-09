using GastosPersonales.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace GastosPersonales.Infrastructure.Persistencia
{
    // Contexto de la base de datos para la aplicación de gastos personales
    public class AplicacionDbContext : DbContext
    {
        public AplicacionDbContext(DbContextOptions<AplicacionDbContext> options) : base(options) { }
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Gasto> Gastos => Set<Gasto>();
        public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<Budget> Budgets => Set<Budget>();
    }
}

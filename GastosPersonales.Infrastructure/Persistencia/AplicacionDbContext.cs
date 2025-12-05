using GastosPersonales.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace GastosPersonales.Infrastructure.Persistencia
{
    public class AplicacionDbContext : DbContext
    {
        public AplicacionDbContext(DbContextOptions<AplicacionDbContext> options) : base(options) { }
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Gasto> Gastos => Set<Gasto>();
        public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();
        public DbSet<Expense> Expenses => Set<Expense>();
    }
}

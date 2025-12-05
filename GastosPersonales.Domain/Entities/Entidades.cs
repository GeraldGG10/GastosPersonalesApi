namespace GastosPersonales.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public string Nombre { get; set; } = "";
        public string Correo { get; set; } = "";
        public string PasswordHash { get; set; } = "";
    }

    public class Categoria : BaseEntity
    {
        public string Nombre { get; set; } = "";
        public int UsuarioId { get; set; }
    }

    public class MetodoPago : BaseEntity
    {
        public string Nombre { get; set; } = "";
        public int UsuarioId { get; set; }
    }

    public class Gasto : BaseEntity
    {
        public decimal Monto { get; set; }
        public string Descripcion { get; set; } = "";
        public System.DateTime Fecha { get; set; }
        public int CategoriaId { get; set; }
        public int UsuarioId { get; set; }
        public int MetodoPagoId { get; set; }
    }
}

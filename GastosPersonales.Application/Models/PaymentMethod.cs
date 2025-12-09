namespace GastosPersonales.Application.Models
{
    // Método de pago asociado a un usuario
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public bool IsActive { get; set; } = true;
        public int UserId { get; set; }
    }
}

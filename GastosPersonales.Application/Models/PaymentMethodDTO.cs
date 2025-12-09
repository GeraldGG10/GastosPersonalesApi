namespace GastosPersonales.Application.Models
{
    // DTO para la transferencia de datos de método de pago
    public class PaymentMethodDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

namespace GastosPersonales.Application.Models
{
    public class PaymentMethodDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

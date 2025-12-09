namespace GastosPersonales.Application.Models
{
    // DTO para la transferencia de datos de gasto
    public class ExpenseDTO
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public int PaymentMethodId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}

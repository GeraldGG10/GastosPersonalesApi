namespace GastosPersonales.Domain.Entities
{
    public class Expense : BaseEntity
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int CategoryId { get; set; }
        public int PaymentMethodId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}

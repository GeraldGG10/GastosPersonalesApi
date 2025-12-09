namespace GastosPersonales.Application.Models
{
    //DTO para la transferencia de datos de presupuesto
    public class BudgetDTO
    {
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}

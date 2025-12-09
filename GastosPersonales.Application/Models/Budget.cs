namespace GastosPersonales.Application.Models
{
    //Presupuesto mensual por categoría
    public class Budget
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public int Month { get; set; } 
        public int Year { get; set; }
        public int UserId { get; set; }
    }
}

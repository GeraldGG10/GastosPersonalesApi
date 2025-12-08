using System.ComponentModel.DataAnnotations.Schema;

namespace GastosPersonales.Domain.Entities
{
    public class Budget : BaseEntity
    {
        public int CategoryId { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        public int Month { get; set; }
        public int Year { get; set; }
        public int UserId { get; set; }
    }
}

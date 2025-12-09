namespace GastosPersonales.Application.Models
{
    // DTO para la transferencia de datos de categoría
    public class CategoryDTO
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}

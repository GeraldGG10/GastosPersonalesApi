namespace GastosPersonales.Application.DTOs
{
    public class ImportResultDTO
    {
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<ImportedExpenseDTO> ImportedExpenses { get; set; } = new List<ImportedExpenseDTO>();
    }

    public class ImportedExpenseDTO
    {
        public int Row { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public int PaymentMethodId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "OK";
        public string? ErrorMessage { get; set; }
    }
}

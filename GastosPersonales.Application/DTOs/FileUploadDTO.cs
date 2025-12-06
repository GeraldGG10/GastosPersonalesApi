namespace GastosPersonales.Application.DTOs
{
    public class FileUploadDTO
    {
        public Stream FileStream { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
    }
}

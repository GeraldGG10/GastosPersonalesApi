namespace GastosPersonales.Application.DTOs
{
    // DTO para la carga de los archivos
    public class FileUploadDTO
    {
        public Stream FileStream { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
    }
}
